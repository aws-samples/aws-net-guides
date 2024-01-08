using DocProcessing.Shared.Model.Textract.QueryAnalysis;
using System.Text.Json;


namespace DocProcessingTest;

[TestClass]
[DeploymentItem(@"TestAssets/TextractResults.json")]
public class TestTextractQuery
{
    private TextractDataModel TextractData { get; set; }
    private TextractAnalysisResult TextractResult { get; set; }

    [TestInitialize()]
    public void Setup()
    {
        using FileStream jsonStream = File.OpenRead(@"TextractResults.json");
        Assert.IsNotNull(jsonStream);

        TextractResult = JsonSerializer.Deserialize<TextractAnalysisResult>(jsonStream);
        Assert.IsNotNull(TextractResult);

        TextractData = new TextractDataModel(TextractResult.Blocks);
    }

    [TestMethod]
    public void GetBlockCount_Count_Valid()
    {
        Assert.IsTrue(TextractResult?.GetBlockCount() == 1000);
    }

    [TestMethod]
    public void GetQueryResults_Count_Valid()
    {
        // Test several patient results from the test file.

        // 1) Confirm that for the patient name query, there are two results
        var queryResultPatientName = TextractData?.GetQueryResults("patientname").ToList();
        Assert.IsTrue(queryResultPatientName?.Count == 2);

        // 2) Confirm the presence of the two well known values for patient name query
        var patientResult1a = queryResultPatientName.Where(a => a.Text == "Edward Sang").ToList();
        Assert.IsTrue(patientResult1a.Count == 1);

        var patientResult1b = queryResultPatientName.Where(a => a.Text == "Denis Roegel").ToList();
        Assert.IsTrue(patientResult1b.Count == 1);

        // Confirm the presence of the two well known values for date of service query
        var queryResultDateOfService = TextractData?.GetQueryResults("dateofservice").ToList();
        Assert.IsTrue(queryResultPatientName?.Count == 2);

        // Confirm the presence of the two well known values for date of service query
        var dateOfServiceResult1a = queryResultDateOfService?.Where(a => a.Text == "20 July 1865").ToList();
        Assert.IsTrue(dateOfServiceResult1a?.Count == 1);

        var dateOfServiceResult1b = queryResultDateOfService?.Where(a => a.Text == "11 january 2021").ToList();
        Assert.IsTrue(dateOfServiceResult1b?.Count == 1);
    }

    [TestMethod]
    public void GetQueryResults_Count_Invalid()
    {
        var queryResultPatientName = TextractData?.GetQueryResults("baadvalue").ToList();
        Assert.IsTrue(queryResultPatientName?.Count == 0);
    }

}