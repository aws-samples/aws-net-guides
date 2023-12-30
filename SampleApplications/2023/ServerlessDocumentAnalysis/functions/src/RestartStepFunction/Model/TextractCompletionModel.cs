using System.Text.Json.Serialization;

namespace RestartStepFunction.Model;

public class TextractCompletionModel
{

    public const string SUCCESS_STATUS = "SUCCEEDED";

    [JsonPropertyName("JobId")]
    public string JobId { get; set; }

    [JsonPropertyName("Status")]
    public string Status { get; set; }

    [JsonPropertyName("API")]
    public string API { get; set; }

    [JsonPropertyName("JobTag")]
    public string JobTag { get; set; }

    [JsonPropertyName("Timestamp")]
    public long Timestamp { get; set; }

    [JsonPropertyName("DocumentLocation")]
    public DocumentLocationModel DocumentLocation { get; set; } = new();

    [JsonIgnore]
    public bool IsSuccess => Status == SUCCESS_STATUS;

}
