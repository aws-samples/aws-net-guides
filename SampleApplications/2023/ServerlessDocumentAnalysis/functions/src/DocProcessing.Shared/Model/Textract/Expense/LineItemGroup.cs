using System.Text.Json.Serialization;

namespace DocProcessing.Shared.Model.Textract.Expense;

public class LineItemGroup
{
    [JsonPropertyName("LineItemGroupIndex")]
    public int? LineItemGroupIndex { get; set; }

    [JsonPropertyName("LineItems")]
    public List<LineItem> LineItems { get; set; }
}