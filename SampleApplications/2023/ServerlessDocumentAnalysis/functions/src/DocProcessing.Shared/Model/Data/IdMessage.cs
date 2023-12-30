using System.Text.Json.Serialization;

namespace DocProcessing.Shared.Model.Data;

public class IdMessage
{
    [JsonPropertyName("id")]
    public virtual string Id { get; set; } = string.Empty;

    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("taskToken")]
    public string TaskToken { get; set; }

    public static IdMessage Create(string id, bool success = true, string message = null) =>
        new()
        {
            Id = id,
            Success = success,
            Message = message
        };
}
