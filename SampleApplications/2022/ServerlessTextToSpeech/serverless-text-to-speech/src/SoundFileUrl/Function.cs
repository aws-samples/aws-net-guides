using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.S3;
using Common;
using Common.Model;

// Bootstrap DI Container.
Bootstrap.ConfigureServices();

// The function handler that will be called for each Lambda event
var handler = async (APIGatewayProxyRequest request, ILambdaContext context) =>
{
    var s3Client = Bootstrap.ServiceProvider.GetRequiredService<IAmazonS3>();
    var dynamoDBContext = Bootstrap.ServiceProvider.GetRequiredService<IDynamoDBContext>();
    if (!int.TryParse(Environment.GetEnvironmentVariable("URL_EXPIRATION"), out int expirationSecond))
    {
        expirationSecond = 30; // 30 seconds by default
    }
    context.Logger.LogInformation("Here ");

    int statusCode = 302;
    Dictionary<string, string> headers = new();
    try
    {
        string id = request.PathParameters["id"];
        var data = await dynamoDBContext.LoadAsync<TextToSpeechModel>(id);
        if (data is null)
        {
            statusCode = 404;
        }
        else
        {
            var url = s3Client.GetPreSignedURL(new()
            {
                BucketName = data.SoundBucket,
                Key = data.SoundKey,
                Expires = DateTime.Now.AddSeconds(expirationSecond)
            });
            headers.Add("Location", url);
            headers.Add("Content-Encoding", "audio/mpeg");
        }
    }
    catch (Exception ex)
    {
        context.Logger.LogError(ex.Message);
        context.Logger.LogError(ex.StackTrace);
        statusCode = 500;
    }

    return new APIGatewayProxyResponse
    {
        StatusCode = statusCode,
        Headers = headers
    };
};

// Build the Lambda runtime client passing in the handler to call for each
// event and the JSON serializer to use for translating Lambda JSON documents
// to .NET types.
await LambdaBootstrapBuilder.Create(handler, new DefaultLambdaJsonSerializer())
        .Build()
        .RunAsync();