using System.Text.Json.Serialization;

namespace ProcessingFunctions.Input;

public class ErrorMessage
{
    [JsonPropertyName("Error")]
    public string Error { get; set; }

    [JsonPropertyName("Cause")]
    public string Cause { get; set; }
}
