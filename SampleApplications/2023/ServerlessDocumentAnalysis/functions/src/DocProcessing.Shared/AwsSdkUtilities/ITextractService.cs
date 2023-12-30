using DocProcessing.Shared.Model.Textract.Expense;
using DocProcessing.Shared.Model.Textract.QueryAnalysis;

namespace DocProcessing.Shared.AwsSdkUtilities;

public interface ITextractService
{
    public Task<TextractDataModel> GetBlocksForAnalysis(string bucket, string key);

    public Task<ExpenseDataModel> GetExpenses(string bucket, string key);

}