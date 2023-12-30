using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.S3;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using AWS.Lambda.Powertools.Logging;
using AWS.Lambda.Powertools.Metrics;
using AWS.Lambda.Powertools.Tracing;
using DocProcessing.Shared.Exceptions;
using DocProcessing.Shared.Model.Data.Query;
using InitializeProcessing.Input;
using ProcessingFunctions.Input;
using ProcessingFunctions.Output;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
[assembly: LambdaGlobalProperties(GenerateMain = true)]
namespace InitializeProcessing;

public class Function(IAmazonS3 s3Client, IDataService dataSvc)
{
    readonly List<string> allowedFileExtensions = [.. (Environment.GetEnvironmentVariable("ALLOWED_FILE_EXTENSIONS") ?? ".pdf").Split(',')];

    static Function()
    {
        AWSSDKHandler.RegisterXRayForAllServices();
    }

    private readonly IAmazonS3 _s3Client = s3Client;
    private readonly IDataService _dataSvc = dataSvc;

    [LambdaFunction]
    [Tracing]
    [Metrics]
    [Logging]
    public async Task<SuccessOutput> SuccessOutputHandler(IdMessage input, ILambdaContext _context)
    {
        var processData = await _dataSvc.GetData<ProcessData>(input.Id).ConfigureAwait(false);
        processData.Success = true;
        await _dataSvc.SaveData(processData).ConfigureAwait(false);

        return new SuccessOutput
        {
            ExternalId = processData.ExternalId,
            Execution = processData.ExecutionId,
            InputKey = processData.InputDocKey,
            InputBucket = processData.InputDocBucket,
            ExpenseReports = processData.ExpenseReports,
            Queries = processData.Queries
        };
    }

    [LambdaFunction]
    [Tracing]
    [Metrics]
    [Logging]
    public async Task<FailOutput> FailOutputHandler(ErrorInput error, ILambdaContext _context)
    {
        var processData = (await _dataSvc.GetBySingleIndex<ProcessData>(error.Execution, "executionIndex").ConfigureAwait(false)).FirstOrDefault();

        return new FailOutput
        {
            Execution = error.Execution,
            ExternalId = processData?.ExternalId,
            InputKey = processData?.InputDocKey,
            InputBucket = processData?.InputDocBucket,
            Error = new ErrorMessage
            {
                Error = error.Error,
                Cause = error.Cause
            }
        };
    }

    [LambdaFunction]
    [Tracing]
    [Metrics]
    [Logging]
    public async Task<IdMessage> InitializeHandler(S3StepFunctionCompositeEvent input, ILambdaContext _context)
    {

        //Initialize the Payload
        ProcessData pl = new(_dataSvc.GenerateId())
        {
            // Get the S3 item to query
            InputDocKey = input.Event.Detail.Object.Key,
            InputDocBucket = input.Event.Detail.Bucket.Name,
            ExecutionId = input.ExecutionId
        };
        pl.FileExtension = Path.GetExtension(pl.InputDocKey);

        // Retreive the Tags for the S3 object
        var data = await _s3Client.GetObjectTaggingAsync(new Amazon.S3.Model.GetObjectTaggingRequest
        {
            BucketName = input.Event.Detail.Bucket.Name,
            Key = input.Event.Detail.Object.Key
        }).ConfigureAwait(false);

        // If there is a tag for queries get them
        var queryTagValue = data.Tagging.GetTagValueList("Queries");
        var queries = await _dataSvc.GetQueries(queryTagValue).ConfigureAwait(false);

        pl.Queries.AddRange(queries.Select(q => new DocumentQuery
        {
            QueryId = q.QueryId,
            QueryText = q.QueryText,
            Processed = false,
            IsValid = true
        }));

        // If there is a tag for external id, get it. Otherwise, we won't use it
        pl.ExternalId = data.Tagging.GetTagValue("Id") ?? Guid.NewGuid().ToString();

        //Save the payload
        await _dataSvc.SaveData(pl).ConfigureAwait(false);

        //Test file extension
        if (!allowedFileExtensions.Contains(pl.FileExtension))
        {
            throw new FileTypeException(pl.Id, $"Invalid file extension: {pl.FileExtension}");
        }

        return IdMessage.Create(pl.Id);
    }
}