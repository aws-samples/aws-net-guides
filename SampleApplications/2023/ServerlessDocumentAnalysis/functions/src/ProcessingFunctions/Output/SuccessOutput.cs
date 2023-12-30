using DocProcessing.Shared.Model.Data.Expense;
using DocProcessing.Shared.Model.Data.Query;
using System.Text.Json.Serialization;

namespace ProcessingFunctions.Output;

public class SuccessOutput : StepFunctionOutputBase
{
    [JsonPropertyName("Success")]
    public override bool Success => true;

    [JsonPropertyName("expenseReports")]
    public List<DocumentExpenseReport> ExpenseReports { get; set; } = [];

    [JsonPropertyName("queries")]
    public List<DocumentQuery> Queries { get; set; } = [];
}
