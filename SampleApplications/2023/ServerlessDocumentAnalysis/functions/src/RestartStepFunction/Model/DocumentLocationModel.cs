using System.Text.Json.Serialization;

namespace RestartStepFunction.Model;

public class DocumentLocationModel
{
    [JsonPropertyName("S3ObjectName")]
    public string S3ObjectName { get; set; }

    [JsonPropertyName("S3Bucket")]
    public string S3Bucket { get; set; }
}
