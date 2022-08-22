using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.SNSEvents;
using Amazon.StepFunctions;
using Common;
using Common.Model;

// Bootstrap DI Container.
Bootstrap.ConfigureServices();

// The function handler that will be called for each Lambda event
var handler = async (SNSEvent snsEvent, ILambdaContext context) =>
{
    var dynamoDBContext = Bootstrap.ServiceProvider.GetRequiredService<IDynamoDBContext>();
    var stepFunctionsCli = Bootstrap.ServiceProvider.GetRequiredService<IAmazonStepFunctions>();
    try
    {
        var model = JsonSerializer.Deserialize<NotifyPollyCompleteModel>(snsEvent.Records[0].Sns.Message.ToString());
        if (model is null)
        {
            throw new ArgumentException("Unable to deserialize SNS Event parameter snsEvent", nameof(snsEvent));
        }

        DynamoDBOperationConfig queryConfig = new ()
        {
            IndexName = "PollyJobId"
        };

        var queryData = dynamoDBContext.QueryAsync<TextToSpeechModel>(model.TaskId, queryConfig);
        var textToSpeechModel = (await queryData.GetNextSetAsync()).FirstOrDefault();

        if (textToSpeechModel is null)
        {
            throw new Exception($"Task not found");
        }

        if (model.TaskStatus != "COMPLETED")
        {
            context.Logger.LogInformation("Sending Failure");
            await stepFunctionsCli.SendTaskFailureAsync(new()
            {
                TaskToken = textToSpeechModel.PollyTaskToken
            });
        }

        // Success. Start up the function again
        context.Logger.LogInformation("Sending Success");
        var jobDataDynamic = new { Payload = textToSpeechModel };
        string jobDataSerialized = JsonSerializer.Serialize(jobDataDynamic);
        await stepFunctionsCli.SendTaskSuccessAsync(new()
        {
            TaskToken = textToSpeechModel.PollyTaskToken,
            Output = jobDataSerialized
        });
    }
    catch (Exception ex)
    {
        context.Logger.LogError(ex.Message);
    }
};

// Build the Lambda runtime client passing in the handler to call for each
// event and the JSON serializer to use for translating Lambda JSON documents
// to .NET types.
await LambdaBootstrapBuilder.Create(handler, new DefaultLambdaJsonSerializer())
        .Build()
        .RunAsync();