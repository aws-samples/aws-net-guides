using System.Text.Json.Serialization;

namespace DocProcessing.Shared.Model.Textract.Expense;
public class Currency
{
    [JsonPropertyName("Code")]
    public string Code { get; set; }
}