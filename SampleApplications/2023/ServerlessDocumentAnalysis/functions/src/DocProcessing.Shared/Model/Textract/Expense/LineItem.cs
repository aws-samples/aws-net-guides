using System.Text.Json.Serialization;

namespace DocProcessing.Shared.Model.Textract.Expense;

public class LineItem
{
    [JsonPropertyName("LineItemExpenseFields")]
    public List<LineItemExpenseField> LineItemExpenseFields { get; set; }
}
