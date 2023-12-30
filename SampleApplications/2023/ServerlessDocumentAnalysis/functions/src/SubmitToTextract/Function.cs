using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Textract;
using Amazon.Textract.Model;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using AWS.Lambda.Powertools.Logging;
using AWS.Lambda.Powertools.Metrics;
using AWS.Lambda.Powertools.Tracing;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
[assembly: LambdaGlobalProperties(GenerateMain = true)]

namespace SubmitToTextract;

public class Function(IAmazonTextract textractClient, IDataService dataService)
{
    private readonly IAmazonTextract _textractClient = textractClient;
    private readonly IDataService _dataService = dataService;

    static Function()
    {
        AWSSDKHandler.RegisterXRayForAllServices();
    }

    [Tracing]
    [Metrics]
    [Logging]
    [LambdaFunction]
    public async Task<IdMessage> SubmitToTextractForStandardAnalysis(IdMessage input, ILambdaContext context)
    {
        var data = await _dataService.GetData<ProcessData>(input.Id).ConfigureAwait(false);
        var textractRequest = new StartDocumentAnalysisRequest
        {
            ClientRequestToken = input.Id,
            JobTag = input.Id,
            FeatureTypes = [FeatureType.TABLES, FeatureType.FORMS],
            NotificationChannel = new Amazon.Textract.Model.NotificationChannel
            {
                SNSTopicArn = Environment.GetEnvironmentVariable("TEXTRACT_TOPIC"),
                RoleArn = Environment.GetEnvironmentVariable("TEXTRACT_ROLE"),
            },
            DocumentLocation = new DocumentLocation
            {
                S3Object = new Amazon.Textract.Model.S3Object
                {
                    Bucket = data.InputDocBucket,
                    Name = data.InputDocKey
                }
            },
            OutputConfig = new OutputConfig
            {
                S3Bucket = Environment.GetEnvironmentVariable("TEXTRACT_BUCKET"),
                S3Prefix = Environment.GetEnvironmentVariable("TEXTRACT_OUTPUT_KEY")
            }
        };

        // Add query config if there are some in the DB
        List<Query> queries = (data.Queries != null
            ? data.Queries.Select(q => new Query
            {
                Alias = q.QueryId,
                Pages = ["*"],
                Text = q.QueryText
            })
            : Enumerable.Empty<Query>()).ToList();
        if (queries.Count != 0)
        {
            textractRequest.FeatureTypes.Add(FeatureType.QUERIES);
            textractRequest.QueriesConfig = new QueriesConfig
            {
                Queries = queries
            };
        }
        else
        {
            Logger.LogInformation("No queries found in the database");
        }

        // Submit to textract for analysis
        var textractResult = await _textractClient.StartDocumentAnalysisAsync(textractRequest).ConfigureAwait(false);
        data.TextractJobId = textractResult.JobId;
        data.TextractTaskToken = input.TaskToken;
        data.TextractOutputKey = $"{Environment.GetEnvironmentVariable("TEXTRACT_OUTPUT_KEY")}/{textractResult.JobId}";
        data.OutputBucket = Environment.GetEnvironmentVariable("TEXTRACT_BUCKET");
        await _dataService.SaveData(data).ConfigureAwait(false);
        return IdMessage.Create(data.Id);
    }

    [Tracing]
    [Metrics]
    [Logging]
    [LambdaFunction]
    public async Task<IdMessage> SubmitToTextractForExpenseAnalysis(IdMessage input, ILambdaContext _context)
    {
        var data = await _dataService.GetData<ProcessData>(input.Id).ConfigureAwait(false);

        var textractRequest = new StartExpenseAnalysisRequest
        {
            ClientRequestToken = input.Id,
            JobTag = input.Id,
            NotificationChannel = new Amazon.Textract.Model.NotificationChannel
            {
                SNSTopicArn = Environment.GetEnvironmentVariable("TEXTRACT_TOPIC"),
                RoleArn = Environment.GetEnvironmentVariable("TEXTRACT_ROLE"),
            },
            DocumentLocation = new DocumentLocation
            {
                S3Object = new Amazon.Textract.Model.S3Object
                {
                    Bucket = data.InputDocBucket,
                    Name = data.InputDocKey
                }
            },
            OutputConfig = new OutputConfig
            {
                S3Bucket = Environment.GetEnvironmentVariable("TEXTRACT_BUCKET"),
                S3Prefix = Environment.GetEnvironmentVariable("TEXTRACT_OUTPUT_KEY")
            }
        };
        var textractResult = await _textractClient.StartExpenseAnalysisAsync(textractRequest).ConfigureAwait(false);
        data.TextractJobId = textractResult.JobId;
        data.TextractTaskToken = input.TaskToken;
        data.ExpenseOutputKey = $"{Environment.GetEnvironmentVariable("TEXTRACT_OUTPUT_KEY")}/{textractResult.JobId}";
        data.OutputBucket = Environment.GetEnvironmentVariable("TEXTRACT_BUCKET");

        await _dataService.SaveData(data).ConfigureAwait(false);

        return IdMessage.Create(data.Id);
    }

}