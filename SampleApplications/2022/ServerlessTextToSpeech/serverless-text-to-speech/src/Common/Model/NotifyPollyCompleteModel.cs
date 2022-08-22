namespace Common.Model;

public class NotifyPollyCompleteModel
{
    [JsonPropertyName("taskId")]
    public string? TaskId { get; set; }

    [JsonPropertyName("taskStatus")]
    public string? TaskStatus { get; set; }

    [JsonPropertyName("outputUri")]
    public string? OutputUri { get; set; }

    [JsonPropertyName("creationTime")]
    public DateTime CreationTime { get; set; }

    [JsonPropertyName("requestCharacters")]
    public int RequestCharacters { get; set; }

    [JsonPropertyName("snsTopicArn")]
    public string? SnsTopicArn { get; set; }

    [JsonPropertyName("outputFormat")]
    public string? OutputFormat { get; set; }

    [JsonPropertyName("textType")]
    public string? TextType { get; set; }

    [JsonPropertyName("voiceId")]
    public string? VoiceId { get; set; }
}
