using Amazon.Lambda.Core;
using Amazon.Lambda.CloudWatchEvents.S3Events;
using System.Text.Json;
// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EventBridgeLambdaAndS3Example;

public class Function
{
    public void FunctionHandler(S3ObjectCreateEvent s3ObjectCreateEvent, ILambdaContext context)
    {
        context.Logger.LogLine("S3 Object Create Event Received");
        context.Logger.LogLine(JsonSerializer.Serialize(s3ObjectCreateEvent));
    }
}
