using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.S3;
using Common;
using Common.Model;

// Bootstrap DI Container.
Bootstrap.ConfigureServices();

// The function handler that will be called for each Lambda event
var handler = async (TextToSpeechModel inputModel, ILambdaContext context) =>
{
    var dynamoDBContext = Bootstrap.ServiceProvider.GetRequiredService<IDynamoDBContext>();
    var s3Client = Bootstrap.ServiceProvider.GetRequiredService<IAmazonS3>();

    // Get the current run's model
    var textToSpeechModel = await dynamoDBContext.LoadAsync<TextToSpeechModel>(inputModel.Id);


    // Delete the source files
    await s3Client.DeleteAsync(textToSpeechModel.BucketName, textToSpeechModel.ObjectKey, null);

    // Update the model
    textToSpeechModel.PollyTaskToken = null;
    textToSpeechModel.PollyJobId = null;
    textToSpeechModel.TaskToken = null;
    textToSpeechModel.TextractJobId = null;


    if (inputModel.PollyOutputUri is null)
    {
        throw new NullReferenceException($"inputModel.{nameof(TextToSpeechModel.PollyOutputUri)} is null");
    }

    Uri uri = new(inputModel.PollyOutputUri);
    // The first segment is the URL, and the second segment is the bucket
    textToSpeechModel.SoundKey = string.Join('/', uri.Segments.Skip(2));

    // Save the data
    await dynamoDBContext.SaveAsync(textToSpeechModel);

    return textToSpeechModel;
};

// Build the Lambda runtime client passing in the handler to call for each
// event and the JSON serializer to use for translating Lambda JSON documents
// to .NET types.
await LambdaBootstrapBuilder.Create(handler, new DefaultLambdaJsonSerializer())
        .Build()
        .RunAsync();