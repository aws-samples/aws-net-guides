using Amazon.DynamoDBv2.DataModel;
using System.Text.Json.Serialization;

namespace DocProcessing.Shared.Model.Data.Expense;

public class DocumentExpenseReport
{
    [JsonPropertyName("expenseGroups")]
    [DynamoDBProperty("expenseGroups")]
    public List<DocumentExpenseGroup> ExpenseGroups { get; set; } = [];

    [JsonPropertyName("scalarSummaryItems")]
    [DynamoDBProperty("scalarSummaryItems")]
    public List<DocumentExpenseSummary> ScalarExpenseSummaryValues { get; set; } = [];

    public void AddScalarExpenseSummaryValue(string currency, string label, string type, string value) =>
        ScalarExpenseSummaryValues.Add(new DocumentExpenseSummary { Currency = currency, Label = label, Type = type, Value = value });







}