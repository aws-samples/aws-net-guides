using Amazon.DynamoDBv2.DataModel;
using System.Text.Json.Serialization;

namespace DocProcessing.Shared.Model.Data.Expense;

public class DocumentExpenseReport()
{
    public DocumentExpenseReport(IEnumerable<DocumentExpenseSummary> summaryItems, IEnumerable<DocumentExpenseGroup> groups)
        : this()
    {
        ExpenseGroups.AddRange(groups);
        ScalarExpenseSummaryValues.AddRange(summaryItems);
    }


    [JsonPropertyName("expenseGroups")]
    [DynamoDBProperty("expenseGroups")]
    public List<DocumentExpenseGroup> ExpenseGroups { get; set; } = [];

    [JsonPropertyName("scalarSummaryItems")]
    [DynamoDBProperty("scalarSummaryItems")]
    public List<DocumentExpenseSummary> ScalarExpenseSummaryValues { get; set; } = [];




}