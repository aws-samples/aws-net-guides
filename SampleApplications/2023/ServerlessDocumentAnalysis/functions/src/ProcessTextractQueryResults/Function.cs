using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using AWS.Lambda.Powertools.Logging;
using AWS.Lambda.Powertools.Metrics;
using AWS.Lambda.Powertools.Tracing;
using DocProcessing.Shared.Model.Data.Query;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
[assembly: LambdaGlobalProperties(GenerateMain = true)]
namespace ProcessTextractQueryResults;

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
        var textractModel = await _textractService.GetBlocksForAnalysis(processData.OutputBucket, processData.TextractOutputKey).ConfigureAwait(false);

        // Get the query Results
        foreach (var query in processData.Queries)
        {
            var queryResult = textractModel.GetQueryResults(query.QueryId);

            if (queryResult.Any())
            {
                query.IsValid = false;
            }
            else
            {
                query.IsValid = true;
                query.Result.AddRange(queryResult.Select(r => new DocumentQueryResult() { Confidence = r.Confidence, ResultText = r.Text }));
            }
        }

        // Save the query results back to the database, and clear the task token
        processData.TextractJobId = null;
        processData.TextractTaskToken = null;
        await _dataService.SaveData(processData).ConfigureAwait(false);

        Logger.LogInformation($"Blocks Found = {textractModel.BlockCount}");

        return input;
    }

}