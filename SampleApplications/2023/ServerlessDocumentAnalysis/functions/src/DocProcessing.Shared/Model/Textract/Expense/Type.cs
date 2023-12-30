using System.Text.Json.Serialization;

namespace DocProcessing.Shared.Model.Textract.Expense;

public class Type
{
    [JsonPropertyName("Text")]
    public string Text { get; set; }

    [JsonPropertyName("Confidence")]
    public double? Confidence { get; set; }
}
