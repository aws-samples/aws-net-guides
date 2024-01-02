using Amazon.Lambda.Core;
using DocProcessing.Shared.AwsSdkUtilities;
using DocProcessing.Shared.Model.Data;
using DocProcessing.Shared.Model.Textract.QueryAnalysis;
using DocProcessing.Shared.Service;
using Moq;
using ProcessTextractQueryResults;
using System.Text.Json;

namespace TestProcessTextractQueryResults;

[TestClass]
[DeploymentItem(@"TestAssets\TextractResults.json")]
public class TestFunctions
{
    readonly Mock<ILambdaContext> _context = new();
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
    public async Task ProcessTextractResults_Query_Valid()
    {
        //Arrange
        IdMessage input = new() { Id = "id" };

        var dataMock = new Mock<IDataService>();
        dataMock.Setup(a => a.GetData<ProcessData>(It.Is<string>(a => a == input.Id)).Result)
            .Returns(new ProcessData { Id = input.Id });

        var textractMock = new Mock<ITextractService>();
        textractMock.Setup(a => a.GetBlocksForAnalysis(It.IsAny<string>(), It.IsAny<string>()).Result)
            .Returns(TextractData);

        var testHandler = new Function(textractMock.Object, dataMock.Object);

        //Act
        var result = await testHandler.FunctionHandler(input, _context.Object);

        //Assert
        Assert.IsTrue(result.Id == input.Id);

    }

}