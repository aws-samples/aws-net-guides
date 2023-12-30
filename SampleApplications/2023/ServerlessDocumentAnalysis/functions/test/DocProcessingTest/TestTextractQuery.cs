using DocProcessing.Shared.Model.Textract.QueryAnalysis;
using System.Text.Json;


namespace DocProcessingTest;

[TestClass]
[DeploymentItem(@"TestAssets\TextractResults.json")]
public class TestTextractQuery
{
    private TextractDataModel? TextractData { get; set; }
    private TextractAnalysisResult? TextractResult { get; set; }

    [TestInitialize()]
    public void Setup()
    {
        using FileStream jsonStream = File.OpenRead(@"TextractResults.json");
        if (jsonStream is null)
        {
            throw new ArgumentNullException(nameof(jsonStream));
        }
        TextractResult = JsonSerializer.Deserialize<TextractAnalysisResult>(jsonStream);
        if (TextractResult is null)
        {
            throw new ArgumentNullException(nameof(TextractResult));
        }
        TextractData = new TextractDataModel(TextractResult.Blocks);
    }


    [TestMethod("Test Block Count")]
    public void TestBlockCount()
    {
        Assert.IsTrue(TextractResult?.GetBlockCount() == 1000);

    }

    [TestMethod("Get Query Result")]
    public void TestQueryResults()
    {
        var queryResultPatientName = TextractData?.GetQueryResults("patientname");

        Assert.IsTrue(queryResultPatientName?.Count() == 2);

        Assert.AreEqual(queryResultPatientName.Where(a => a.Text == "Edward Sang").Count(), 1);

        Assert.AreEqual(queryResultPatientName.Where(a => a.Text == "Denis Roegel").Count(), 1);

        var queryResultDateOfService = TextractData?.GetQueryResults("dateofservice");

        Assert.AreEqual(queryResultDateOfService?.Where(a => a.Text == "20 July 1865").Count(), 1);

        Assert.AreEqual(queryResultDateOfService?.Where(a => a.Text == "11 january 2021").Count(), 1);
    }

    [TestMethod("Get Invalid Query Result")]
    public void TestInvalidQueryResults()
    {
        var queryResultPatientName = TextractData?.GetQueryResults("baadvalue");

        Assert.IsTrue(queryResultPatientName?.Count() == 0);
    }

}