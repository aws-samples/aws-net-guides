using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using AWS.Lambda.Powertools.Logging;
using AWS.Lambda.Powertools.Metrics;
using AWS.Lambda.Powertools.Tracing;
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
    public async Task<IdMessage> FunctionHandler(IdMessage input, ILambdaContext _context)
    {
        var processData = await _dataService.GetData<ProcessData>(input.Id).ConfigureAwait(false);

        // Get the step functions Result
        var textractExpenseModel = await _textractService.GetExpenses(processData.OutputBucket, processData.ExpenseOutputKey).ConfigureAwait(false);

        // Get Expense Data and save to the DB
        foreach (var id in textractExpenseModel.GetExpenseReportIndexes())
        {
            var report = new DocumentExpenseReport();

            //Add Scalar Values            
            foreach (var scalarSummaryField in textractExpenseModel.GetScalarSummaryFields(id))
            {
                report.AddScalarExpenseSummaryValue(
                    scalarSummaryField?.Currency?.Code,
                    scalarSummaryField?.LabelDetection?.Text,
                    scalarSummaryField?.Type?.Text,
                    scalarSummaryField?.ValueDetection?.Text);
            };

            // Add Groups
            foreach (var groupType in textractExpenseModel.GetGroupTypes(id))
            {
                var (group, type) = groupType;
                var groupSummaryFields = textractExpenseModel.GetGroupSummaryFields(id, group, type);

                var documentExpenseGroup = new DocumentExpenseGroup
                {
                    Group = group,
                    Type = type
                };
                foreach (var groupSummaryField in groupSummaryFields)
                {
                    documentExpenseGroup.GroupSummaryItems.Add(new DocumentExpenseSummary
                    {
                        Currency = groupSummaryField?.Currency?.Code,
                        Label = groupSummaryField?.LabelDetection?.Text,
                        Type = groupSummaryField?.Type?.Text,
                        Value = groupSummaryField?.ValueDetection?.Text
                    });
                }
                // Add to the document expense report
                report.ExpenseGroups.Add(documentExpenseGroup);
            };
            processData.ExpenseReports.Add(report);
        }

        // Save the query results back to the database, and clear the task token
        processData.TextractJobId = null;
        processData.TextractTaskToken = null;
        await _dataService.SaveData(processData).ConfigureAwait(false);
        return input;
    }
}
