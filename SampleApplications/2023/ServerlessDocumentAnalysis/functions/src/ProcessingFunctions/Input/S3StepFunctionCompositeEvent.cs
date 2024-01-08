using Amazon.Lambda.CloudWatchEvents.S3Events;
using DocProcessing.Shared.Model;
using System.Text.Json.Serialization;

namespace InitializeProcessing.Input;

public class S3StepFunctionCompositeEvent : IProcessDataInitializer
{
    [JsonPropertyName("Event")]
    public S3ObjectCreateEvent Event { get; set; }

    [JsonPropertyName("ExecutionId")]
    public string ExecutionId { get; set; }

    string IProcessDataInitializer.ExecutionId => ExecutionId;

    string IProcessDataInitializer.BucketName => Event.Detail.Bucket.Name;

    string IProcessDataInitializer.Key => Event.Detail.Object.Key;

    string IProcessDataInitializer.FileExtension => Path.GetExtension(Event.Detail.Object.Key);
}
