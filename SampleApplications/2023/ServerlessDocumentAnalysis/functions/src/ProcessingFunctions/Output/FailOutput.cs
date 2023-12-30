using ProcessingFunctions.Input;
using System.Text.Json.Serialization;

namespace ProcessingFunctions.Output;

public class FailOutput : StepFunctionOutputBase
{
    [JsonPropertyName("Success")]
    public override bool Success => false;

    [JsonPropertyName("Error")]
    public ErrorMessage Error { get; set; }

}
