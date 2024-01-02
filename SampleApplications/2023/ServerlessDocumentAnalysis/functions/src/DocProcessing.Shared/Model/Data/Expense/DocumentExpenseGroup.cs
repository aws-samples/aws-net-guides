using Amazon.DynamoDBv2.DataModel;
using System.Text.Json.Serialization;

namespace DocProcessing.Shared.Model.Data.Expense;

public class DocumentExpenseGroup()
{
    public DocumentExpenseGroup(string group, string type, IEnumerable<DocumentExpenseSummary> summaryItems)
        : this()
    {
        Group = group;
        Type = type;
        GroupSummaryItems.AddRange(summaryItems);
    }

    [JsonPropertyName("group")]
    [DynamoDBProperty("group")]
    public string Group { get; set; }

    [JsonPropertyName("type")]
    [DynamoDBProperty("type")]
    public string Type { get; set; }

    [JsonPropertyName("summaryItems")]
    [DynamoDBProperty("summaryItems")]
    public List<DocumentExpenseSummary> GroupSummaryItems { get; set; } = [];
}

