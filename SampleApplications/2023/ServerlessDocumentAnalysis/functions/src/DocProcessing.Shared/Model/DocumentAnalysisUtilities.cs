using DocProcessing.Shared.Model.Data.Expense;
using DocProcessing.Shared.Model.Data.Query;
using DocProcessing.Shared.Model.Textract.Expense;
using DocProcessing.Shared.Model.Textract.QueryAnalysis;
using Block = DocProcessing.Shared.Model.Textract.QueryAnalysis.Block;

namespace DocProcessing.Shared.Model;
public static class DocumentAnalysisUtilities
{
    public static IEnumerable<DocumentExpenseGroup> GetDocumentExpenseGroups(ExpenseDataModel data, int id)
    {
        var groupTypes = data.GetGroupTypes(id);
        foreach (var (group, type) in groupTypes)
        {
            var fields = data.GetGroupSummaryFields(id, group, type).Select(CreateDocumentExpenseSummary);
            yield return new DocumentExpenseGroup(group, type, fields);
        }
    }

    public static IEnumerable<DocumentExpenseSummary> GetExpenseSummaryFields(ExpenseDataModel data, int id) =>
        data.GetScalarSummaryFields(id).Select(CreateDocumentExpenseSummary);

    public static IEnumerable<DocumentQueryResult> GetDocumentQueryResults(TextractDataModel data, string queryId) =>
         data.GetQueryResults(queryId).Select(CreateDocumentQueryResult);

    private static DocumentQueryResult CreateDocumentQueryResult(Block block) =>
        new()
        {
            Confidence = block.Confidence,
            ResultText = block.Text
        };

    private static DocumentExpenseSummary CreateDocumentExpenseSummary(SummaryField field) =>
        new()
        {
            Currency = field.Currency?.Code,
            Label = field.LabelDetection?.Text,
            Type = field.Type?.Text,
            Value = field.ValueDetection?.Text
        };
}
