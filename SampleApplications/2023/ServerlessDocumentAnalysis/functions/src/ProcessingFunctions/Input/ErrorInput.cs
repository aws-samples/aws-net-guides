using System.Text.Json.Serialization;

namespace ProcessingFunctions.Input;

public class ErrorInput
{
    [JsonPropertyName("execution")]
    public string Execution { get; set; }

    [JsonPropertyName("error")]
    public string Error { get; set; }

    [JsonPropertyName("cause")]
    public string Cause { get; set; }
}
