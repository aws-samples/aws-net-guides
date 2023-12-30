using Amazon.Lambda.CloudWatchEvents.S3Events;
using System.Text.Json.Serialization;

namespace InitializeProcessing.Input;

public class S3StepFunctionCompositeEvent
{
    [JsonPropertyName("Event")]
    public S3ObjectCreateEvent Event { get; set; }

    [JsonPropertyName("ExecutionId")]
    public string ExecutionId { get; set; }
}
