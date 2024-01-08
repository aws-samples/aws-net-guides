using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using AWS.Lambda.Powertools.Logging;
using AWS.Lambda.Powertools.Metrics;
using AWS.Lambda.Powertools.Tracing;
using DocProcessing.Shared.Model;
using DocProcessing.Shared.Model.Data.Expense;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
[assembly: LambdaGlobalProperties(GenerateMain = true)]

namespace ProcessTextractExpenseResults;

public class Function(ITextractService textractService, IDataService dataService)
{

    private readonly ITextractService _textractService = textractService;
    private readonly IDataService _dataService = dataService;

    static Function()
    {
        AWSSDKHandler.RegisterXRayForAllServices();
    }

    [Tracing]
    [Metrics]
    [Logging]
    [LambdaFunction]
    public async Task<IdMessage> FunctionHandler(IdMessage input, ILambdaContext context)
    {
        var processData = await _dataService.GetData<ProcessData>(input.Id).ConfigureAwait(false);

        // Get the step functions Result
        var textractExpenseModel = await _textractService.GetExpenses(processData.OutputBucket, processData.ExpenseOutputKey).ConfigureAwait(false);

        // Compose the expense reports and add to the database
        foreach (var id in textractExpenseModel.GetExpenseReportIndexes())
        {
            var summaryFields = DocumentAnalysisUtilities.GetExpenseSummaryFields(textractExpenseModel, id);
            var groupSummaryFields = DocumentAnalysisUtilities.GetDocumentExpenseGroups(textractExpenseModel, id);
            processData.ExpenseReports.Add(new DocumentExpenseReport(summaryFields, groupSummaryFields));
        }

        // Save the query results back to the database, and clear the task token
        processData.ClearTextractJobData();
        await _dataService.SaveData(processData).ConfigureAwait(false);
        return input;
    }
}
