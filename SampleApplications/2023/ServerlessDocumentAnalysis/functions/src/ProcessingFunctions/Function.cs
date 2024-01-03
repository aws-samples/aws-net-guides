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

public class Function(IDataService dataSvc)
{
    private const string QUERY_TAG = "Queries";
    private const string ID_TAG = "Id";

    readonly List<string> _allowedFileExtensions = [.. (Environment.GetEnvironmentVariable("ALLOWED_FILE_EXTENSIONS") ?? ".pdf").Split(',')];

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
        ProcessData pl = await _dataSvc.InitializeProcessData(input, ID_TAG, QUERY_TAG).ConfigureAwait(false);

        //Save the payload to the DynamoDB table
        await _dataSvc.SaveData(pl).ConfigureAwait(false);

        //Ensure that we have a valid file extension. If not, we will throw a FileTypeExtension that will be
        // caught by the step function
        if (!_allowedFileExtensions.Contains(pl.FileExtension))
        {
            throw new FileTypeException(pl.Id, $"Invalid file extension: {pl.FileExtension}");
        }

        return IdMessage.Create(pl.Id);
    }
}