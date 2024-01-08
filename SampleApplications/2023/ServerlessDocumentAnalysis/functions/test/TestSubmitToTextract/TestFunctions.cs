using Amazon.Lambda.Core;
using Amazon.Textract;
using Amazon.Textract.Model;
using DocProcessing.Shared.Model.Data;
using DocProcessing.Shared.Service;
using Moq;
using SubmitToTextract;

namespace TestSubmitToTextract;

[TestClass]
public class TestFunctions
{
    Mock<IAmazonTextract> _amazonTextractMock;
    readonly Mock<ILambdaContext> _context = new();

    [TestInitialize]
    public void Setup()
    {
        _amazonTextractMock = new Mock<IAmazonTextract>();
        _amazonTextractMock.Setup(a => a.StartDocumentAnalysisAsync(It.IsAny<StartDocumentAnalysisRequest>(), new CancellationToken()).Result)
            .Returns(new StartDocumentAnalysisResponse()
            {
                JobId = "xxxx123",
            });

        _amazonTextractMock.Setup(a => a.StartExpenseAnalysisAsync(It.IsAny<StartExpenseAnalysisRequest>(), new CancellationToken()).Result)
        .Returns(new StartExpenseAnalysisResponse()
        {
            JobId = "xxxx123"
        });

        Environment.SetEnvironmentVariable("TEXTRACT_BUCKET", "textract-bucket");
        Environment.SetEnvironmentVariable("TEXTRACT_OUTPUT_KEY", "textractKey");
        Environment.SetEnvironmentVariable("TEXTRACT_TOPIC", "textractTopic");
        Environment.SetEnvironmentVariable("TEXTRACT_ROLE", "textractRole");

    }

    [TestMethod]
    public async Task SubmitToTextractForStandardAnalysis_Id_Valid()
    {
        //Arrange
        Mock<IDataService> dataMock = new();
        dataMock.Setup(a => a.GetData<ProcessData>(It.Is<string>(a => a == "id")).Result)
        .Returns(new ProcessData()
        {
            Id = "id",
            InputDocBucket = "bucket",
            InputDocKey = "key",
            Queries =
            [
                new()
                {
                    IsValid = false,
                    Processed = false,
                    QueryId = "qid",
                    QueryText = "What is the answer?"
                }
            ]
        });

        IdMessage input = new() { Id = "id" };
        var testHandler = new Function(_amazonTextractMock.Object, dataMock.Object);

        //Act
        var data = await testHandler.SubmitToTextractForStandardAnalysis(input, _context.Object);

        //Assert
        Assert.IsTrue(data.Id == input.Id);
    }

    [TestMethod]
    public async Task SubmitToTextractForExpenseAnalysis_Id_Valid()
    {
        //Arrange
        Mock<IDataService> dataMock = new();
        dataMock.Setup(a => a.GetData<ProcessData>(It.Is<string>(a => a == "id")).Result)
        .Returns(new ProcessData()
        {
            Id = "id",
            InputDocBucket = "bucket",
            InputDocKey = "key",
        });

        IdMessage input = new() { Id = "id" };
        var testHandler = new Function(_amazonTextractMock.Object, dataMock.Object);

        //Act
        var data = await testHandler.SubmitToTextractForExpenseAnalysis(input, _context.Object);

        //Assert
        Assert.IsTrue(data.Id == input.Id);
    }
}