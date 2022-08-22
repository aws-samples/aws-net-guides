using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.S3;
using Amazon.Textract;
using Common;
using Common.Model;
using System.Text.Json;

// Bootstrap DI Container.
Bootstrap.ConfigureServices();


// The function handler that will be called for each Lambda event
var handler = async (TextToSpeechModel inputModel, ILambdaContext context) =>
{
    // Get Required Services
    var textractCli = Bootstrap.ServiceProvider.GetRequiredService<IAmazonTextract>();
    var dynamoDBContext = Bootstrap.ServiceProvider.GetRequiredService<IDynamoDBContext>();
    var s3Cli = Bootstrap.ServiceProvider.GetRequiredService<IAmazonS3>();

    // Get tags. If there is an "ID" tag on the object, then use it as the ID. Otherwise, use the provided one.
    var itemTags = await s3Cli.GetObjectTaggingAsync(new()
    {
        BucketName = inputModel.BucketName,
        Key = inputModel.ObjectKey
    });

    var id = itemTags.Tagging.FirstOrDefault(t => t.Key == Environment.GetEnvironmentVariable("ID_KEY"));

    if (id?.Value is not null)
    {
        // If we supplied an tag on the S3 Object corresponding to the desired id, we'll use that. Otherwise we'll use the supplied id
        inputModel.Id = id.Value;
    }

    //Submit to Textract
    var startDocProcessResult = await textractCli.StartDocumentTextDetectionAsync(new()
    {
        DocumentLocation = new()
        {
            S3Object = new()
            {
                Bucket = inputModel.BucketName,
                Name = inputModel.ObjectKey
            }
        },
        OutputConfig = new()
        {
            S3Bucket = Environment.GetEnvironmentVariable("OUTPUT_BUCKET"),
            S3Prefix = Environment.GetEnvironmentVariable("OUTPUT_PREFIX")
        },
        NotificationChannel = new()
        {
            SNSTopicArn = Environment.GetEnvironmentVariable("TEXTRACT_TOPIC"),
            RoleArn = Environment.GetEnvironmentVariable("TEXTRACT_ROLE")
        },
        JobTag = inputModel.Id
    });


    inputModel.TextractJobId = startDocProcessResult.JobId;

    //Save the data to the DB

    await dynamoDBContext.SaveAsync(inputModel);

    return inputModel;
};

// Build the Lambda runtime client passing in the handler to call for each
// event and the JSON serializer to use for translating Lambda JSON documents
// to .NET types
await LambdaBootstrapBuilder.Create(handler, new DefaultLambdaJsonSerializer())
        .Build()
        .RunAsync();