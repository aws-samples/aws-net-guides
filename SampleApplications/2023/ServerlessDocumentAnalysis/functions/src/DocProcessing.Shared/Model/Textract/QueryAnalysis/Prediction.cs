using System.Text.Json.Serialization;

namespace DocProcessing.Shared.Model.Textract.QueryAnalysis;
public class Prediction
{
    [JsonPropertyName("Confidence")]
    public double? Confidence { get; set; }

    [JsonPropertyName("Value")]
    public string Value { get; set; }

}
