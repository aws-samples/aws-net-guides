namespace Common.Model;

public class NotifyTextractCompleteModel
{
    [JsonPropertyName("JobId")]
    public string? JobId { get; set; }

    [JsonPropertyName("Status")]
    public string? Status { get; set; }

    [JsonPropertyName("API")]
    public string? API { get; set; }

    [JsonPropertyName("JobTag")]
    public string? JobTag { get; set; }

    [JsonPropertyName("Timestamp")]
    public long? Timestamp { get; set; }

    [JsonPropertyName("DocumentLocation")]
    public DocumentLocation? DocumentLocation { get; set; }
}
