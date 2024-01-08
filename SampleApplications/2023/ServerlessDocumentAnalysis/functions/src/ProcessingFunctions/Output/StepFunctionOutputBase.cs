using System.Text.Json.Serialization;

namespace ProcessingFunctions.Output;

public abstract class StepFunctionOutputBase
{

    public abstract bool Success { get; }

    [JsonPropertyName("ExternalId")]
    public string ExternalId { get; set; }

    [JsonPropertyName("Execution")]
    public string Execution { get; set; }

    [JsonPropertyName("InputBucket")]
    public string InputBucket { get; set; }

    [JsonPropertyName("InputKey")]
    public string InputKey { get; set; }

}
