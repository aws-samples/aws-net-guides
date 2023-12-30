using System.Text.Json.Serialization;

namespace DocProcessing.Shared.Model.Textract.Expense;

public class ValueDetection
{
    [JsonPropertyName("Text")]
    public string Text { get; set; }

    [JsonPropertyName("Geometry")]
    public Geometry Geometry { get; set; }

    [JsonPropertyName("Confidence")]
    public double? Confidence { get; set; }
}