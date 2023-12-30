using Amazon.DynamoDBv2.DataModel;
using System.Text.Json.Serialization;

namespace DocProcessing.Shared.Model.Data.Expense;
public class DocumentExpenseValue
{

    [DynamoDBProperty("Text")]
    [JsonPropertyName("Text")]
    public string Text { get; set; }

    [DynamoDBProperty("Confidence")]
    [JsonPropertyName("Confidence")]
    public double? Confidence { get; set; }

}
