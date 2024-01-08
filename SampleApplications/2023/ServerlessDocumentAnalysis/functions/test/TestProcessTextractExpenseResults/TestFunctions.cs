using Amazon.Lambda.Core;
using DocProcessing.Shared.AwsSdkUtilities;
using DocProcessing.Shared.Model.Data;
using DocProcessing.Shared.Model.Textract.Expense;
using DocProcessing.Shared.Service;
using ProcessTextractExpenseResults;
using System.Text.Json;

namespace TestProcessTextractExpenseResults;

[TestClass]
[DeploymentItem(@"TestAssets/ExpenseAnalysis.json")]
public class TestFunctions
{
    readonly Mock<ILambdaContext> _context = new();
    private ExpenseResult ExpenseResult { get; set; }
    private ExpenseDataModel ExpenseData { get; set; }

    [TestInitialize()]
    public void Setup()
    {
        using FileStream jsonStream = File.OpenRead(@"ExpenseAnalysis.json");
        Assert.IsNotNull(jsonStream);

        ExpenseResult = JsonSerializer.Deserialize<ExpenseResult>(jsonStream);
        Assert.IsNotNull(ExpenseResult);

        ExpenseData = new ExpenseDataModel(ExpenseResult.ExpenseDocuments);
    }

    [TestMethod]
    public async Task ProcessTextractResults_Expense_Valid()
    {
        //Arrange
        IdMessage input = new() { Id = "id" };

        var dataMock = new Mock<IDataService>();
        dataMock.Setup(a => a.GetData<ProcessData>(It.Is<string>(a => a == input.Id)).Result)
            .Returns(new ProcessData { Id = input.Id });

        var textractMock = new Mock<ITextractService>();
        textractMock.Setup(a => a.GetExpenses(It.IsAny<string>(), It.IsAny<string>()).Result)
            .Returns(ExpenseData);

        var testHandler = new Function(textractMock.Object, dataMock.Object);

        //Act
        var result = await testHandler.FunctionHandler(input, _context.Object);

        //Assert
        Assert.IsTrue(result.Id == input.Id);
    }


}