using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Polly;
using Amazon.Textract;
using Amazon.Textract.Model;
using Common;
using Common.Model;
using Common.Services;

// Bootstrap DI Container.
Bootstrap.ConfigureServices();

// The function handler that will be called for each Lambda event
var handler = async (TextToSpeechModel inputModel, ILambdaContext context) =>
{
    var textractCli = Bootstrap.ServiceProvider.GetRequiredService<IAmazonTextract>();
    var pollyCli = Bootstrap.ServiceProvider.GetRequiredService<IAmazonPolly>();
    var textractUtil = Bootstrap.ServiceProvider.GetRequiredService<ITextToSpeechUtilities>();
    var dynamoDBContext = Bootstrap.ServiceProvider.GetRequiredService<IDynamoDBContext>();

    //Get The Model
    var textToSpeechModel = await dynamoDBContext.LoadAsync<TextToSpeechModel>(inputModel.Id);
    textToSpeechModel.PollyTaskToken = inputModel.PollyTaskToken;

    //Retrieve the data from Textract
    List<Block> resultBlocks = new();
    string? token = null;
    do
    {
        var data = await textractCli.GetDocumentTextDetectionAsync(new()
        {
            JobId = textToSpeechModel.TextractJobId,
            MaxResults = 1000,
            NextToken = token
        });
        token = data.NextToken;
        resultBlocks.AddRange(data.Blocks);
    } while (token is not null);

    // Send to Polly
    context.Logger.LogInformation("Done Retrieving Textract. Sending to Polly");

    var doc = textractUtil.GetTextDocument(resultBlocks);

    var pollyStartResult = await pollyCli.StartSpeechSynthesisTaskAsync(new()
    {
        OutputFormat = OutputFormat.Mp3,
        OutputS3BucketName = Environment.GetEnvironmentVariable("SOUND_BUCKET"),
        OutputS3KeyPrefix = Environment.GetEnvironmentVariable("SOUND_PREFIX"),
        Text = doc.GetDocument(),
        SnsTopicArn = Environment.GetEnvironmentVariable("POLLY_TOPIC"),
        VoiceId = VoiceId.FindValue(Environment.GetEnvironmentVariable("POLLY_VOICE"))
    });

    textToSpeechModel.PollyJobId = pollyStartResult.SynthesisTask.TaskId;
    textToSpeechModel.PollyOutputUri = pollyStartResult.SynthesisTask.OutputUri;
    textToSpeechModel.SoundBucket = Environment.GetEnvironmentVariable("SOUND_BUCKET");

    // Save the data out to DynamoDB
    await dynamoDBContext.SaveAsync(textToSpeechModel);
};



// Build the Lambda runtime client passing in the handler to call for each
// event and the JSON serializer to use for translating Lambda JSON documents
// to .NET types.
await LambdaBootstrapBuilder.Create(handler, new DefaultLambdaJsonSerializer())
        .Build()
        .RunAsync();