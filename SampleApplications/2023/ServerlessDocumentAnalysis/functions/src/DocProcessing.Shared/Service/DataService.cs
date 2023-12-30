using Amazon.DynamoDBv2.DataModel;
using DocProcessing.Shared.Model.Data.Query;

namespace DocProcessing.Shared.Service;

public class DataService(IDynamoDBContext dbContext) : IDataService
{
    IDynamoDBContext DbContext { get; } = dbContext;

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
}
