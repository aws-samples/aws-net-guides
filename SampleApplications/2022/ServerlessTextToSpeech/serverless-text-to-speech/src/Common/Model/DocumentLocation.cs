
namespace Common.Model;

public class DocumentLocation
{
    [JsonPropertyName("S3ObjectName")]
    public string? S3ObjectName { get; set; }

    [JsonPropertyName("S3Bucket")]
    public string? S3Bucket { get; set; }
}
