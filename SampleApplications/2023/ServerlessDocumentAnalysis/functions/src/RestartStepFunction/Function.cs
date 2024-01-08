using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.SNSEvents;
using Amazon.Runtime;
using Amazon.StepFunctions;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using AWS.Lambda.Powertools.Logging;
using AWS.Lambda.Powertools.Metrics;
using AWS.Lambda.Powertools.Tracing;
using RestartStepFunction.Exceptions;
using RestartStepFunction.Model;
using System.Text.Json;

//Configure the Serializer
[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
[assembly: LambdaGlobalProperties(GenerateMain = true)]
namespace RestartStepFunction;

public class Function(IAmazonStepFunctions stepFunctionClient, IDataService dataService)
{
    private readonly IAmazonStepFunctions _stepFunctionClient = stepFunctionClient;
    private readonly IDataService _dataService = dataService;

    static Function()
    {
        AWSSDKHandler.RegisterXRayForAllServices();
    }

    [Tracing]
    [Metrics]
    [Logging]
    [LambdaFunction]
    public async Task FunctionHandler(SNSEvent input, ILambdaContext context)
    {
        var record = input.Records.FirstOrDefault() ?? throw new RestartStepFunctionException("No message received");

        // Deserialize the Message
        var message = JsonSerializer.Deserialize<TextractCompletionModel>(record.Sns.Message) ?? throw new RestartStepFunctionException($"Completion Message is Null");

        // Get the Task Token
        var processData = await _dataService.GetData<ProcessData>(message.JobTag).ConfigureAwait(false);

        if (processData.TextractTaskToken is null)
        {
            throw new RestartStepFunctionException("Missing Task Token");
        }

        var responseMessage = new IdMessage
        {
            Id = message.JobTag
        };

        AmazonWebServiceResponse response;

        if (message.IsSuccess)
        {
            response = await _stepFunctionClient.SendTaskSuccessAsync(new()
            {
                TaskToken = processData.TextractTaskToken,
                Output = JsonSerializer.Serialize(responseMessage)
            }).ConfigureAwait(false);
        }
        else
        {
            response = await _stepFunctionClient.SendTaskFailureAsync(new()
            {
                TaskToken = processData.TextractTaskToken,
                Error = $"{message.API} {message.Status}",
                Cause = record.Sns.Message
            }).ConfigureAwait(false);
        }

        // Log the output
        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
            Logger.LogInformation($"Successfully sent Step Function Completion");
            Logger.LogInformation(response);
        }
        else
        {
            Logger.LogError($"Error sending Step Function Completion");
            Logger.LogError(response);
        }

        await _dataService.SaveData(processData).ConfigureAwait(false);
    }
}