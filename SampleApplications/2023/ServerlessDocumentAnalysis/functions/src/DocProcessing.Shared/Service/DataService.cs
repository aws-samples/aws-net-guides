using Amazon.DynamoDBv2.DataModel;
using Amazon.S3;
using DocProcessing.Shared.AwsSdkUtilities;
using DocProcessing.Shared.Model;
using DocProcessing.Shared.Model.Data;
using DocProcessing.Shared.Model.Data.Query;

namespace DocProcessing.Shared.Service;

public class DataService(IDynamoDBContext dbContext, IAmazonS3 s3Client) : IDataService
{
    IDynamoDBContext DbContext { get; } = dbContext;
    IAmazonS3 S3Client { get; } = s3Client;

    public async Task<IEnumerable<DocumentQuery>> GetAllQueries()
    {
        var asyncData = DbContext.ScanAsync<DocumentQuery>(Enumerable.Empty<ScanCondition>());
        return await asyncData.GetRemainingAsync().ConfigureAwait(false);
    }



    public async Task<IEnumerable<DocumentQuery>> GetQueries(IEnumerable<string> queryKeys)
    {
        if (queryKeys is null || !queryKeys.Any())
        {
            return await GetAllQueries().ConfigureAwait(false);
        }

        var batchGet = DbContext.CreateBatchGet<DocumentQuery>();
        foreach (var key in queryKeys) { batchGet.AddKey(key); }

        await batchGet.ExecuteAsync().ConfigureAwait(false);

        return batchGet.Results;

    }

    public string GenerateId(string id = null)
    {
        return id ?? Guid.NewGuid().ToString("N");
    }

    public async Task<T> SaveData<T>(T data)
    {
        await DbContext.SaveAsync(data).ConfigureAwait(false);

        return data;
    }

    public async Task<T> GetData<T>(string id)
    {
        return await DbContext.LoadAsync<T>(id).ConfigureAwait(false);
    }

    public async Task<List<T>> GetBySingleIndex<T>(string id, string indexName)
    {            
        return await DbContext.QueryAsync<T>(id, new DynamoDBOperationConfig
        {
            IndexName = indexName,                                
        }).GetRemainingAsync().ConfigureAwait(false);
    }


    // This method initializes an initialized empty process data object, based on the S3 object that is uploaded
    public async Task<ProcessData> InitializeProcessData(IProcessDataInitializer initializer, string idTagKey = null, string queryTagKey = null)
    {
        // Retreive the Tags for the S3 object
        var data = await S3Client.GetObjectTaggingAsync(new Amazon.S3.Model.GetObjectTaggingRequest
        {
            BucketName = initializer.BucketName,
            Key = initializer.Key
        }).ConfigureAwait(false);
        var queryTagValue = data.Tagging.GetTagValueList(queryTagKey);
        var idTagValue = data.Tagging.GetTagValue(idTagKey);

        ProcessData processData = new()
        {
            Id = GenerateId(),
            ExecutionId = initializer.ExecutionId,
            InputDocBucket = initializer.BucketName,
            InputDocKey = initializer.Key,
            FileExtension = initializer.FileExtension,
            ExternalId = idTagValue ?? Guid.NewGuid().ToString()
        };

        // Populate queries
        var queries = await GetQueries(queryTagValue).ConfigureAwait(false);
        processData.PopulateQueries(queries);

        return processData;

    }



}
