using System.Text.Json.Serialization;

namespace DocProcessing.Shared.Model.Textract.Expense;

public class ExpenseResult
{
    [JsonPropertyName("DocumentMetadata")]
    public DocumentMetadata DocumentMetadata { get; set; }

    [JsonPropertyName("ExpenseDocuments")]
    public List<ExpenseDocument> ExpenseDocuments { get; set; }
}
