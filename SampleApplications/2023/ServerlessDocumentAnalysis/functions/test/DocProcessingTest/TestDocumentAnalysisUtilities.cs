using DocProcessing.Shared.Model;
using DocProcessing.Shared.Model.Textract.Expense;
using DocProcessing.Shared.Model.Textract.QueryAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DocProcessing.Shared.Test;

[TestClass]
[DeploymentItem(@"TestAssets\TextractResults.json")]
[DeploymentItem(@"TestAssets\ExpenseAnalysis.json")]
public class TestDocumentAnalysisUtilities
{
    private ExpenseResult ExpenseResult { get; set; }
    private ExpenseDataModel ExpenseData { get; set; }
    private TextractDataModel TextractData { get; set; }
    private TextractAnalysisResult TextractResult { get; set; }

    [TestInitialize()]
    public void Setup()
    {
        using FileStream jsonStream = File.OpenRead(@"ExpenseAnalysis.json");
        Assert.IsNotNull(jsonStream);

        ExpenseResult = JsonSerializer.Deserialize<ExpenseResult>(jsonStream);
        Assert.IsNotNull(ExpenseResult);

        ExpenseData = new ExpenseDataModel(ExpenseResult.ExpenseDocuments);

        using FileStream textStream = File.OpenRead(@"TextractResults.json");
        Assert.IsNotNull(textStream);

        TextractResult = JsonSerializer.Deserialize<TextractAnalysisResult>(textStream);
        Assert.IsNotNull(TextractResult);

        TextractData = new TextractDataModel(TextractResult.Blocks);
    }

    [TestMethod]
    public void GetDocumentExpenseGroups_Id_Valid()
    {
        // Act
        var data = DocumentAnalysisUtilities.GetDocumentExpenseGroups(ExpenseData, 1).ToList();

        // Asseert
        Assert.IsTrue(data.Count == 2);
    }

    [TestMethod]
    public void GetDocumentExpenseGroups_Id_Invalid()
    {
        // Act
        var data = DocumentAnalysisUtilities.GetDocumentExpenseGroups(ExpenseData, 3).ToList();

        // Asseert
        Assert.IsTrue(data.Count == 0);
    }

    [TestMethod]
    public void GetExpenseSummaryFields_Id_Valid()
    {
        // Act
        var data = DocumentAnalysisUtilities.GetExpenseSummaryFields(ExpenseData, 1).ToList();

        //Assert
        Assert.IsTrue(data.Count == 18);
    }

    [TestMethod]
    public void GetExpenseSummaryFields_Id_Invalid()
    {
        // Act
        var data = DocumentAnalysisUtilities.GetExpenseSummaryFields(ExpenseData, 3).ToList();

        //Assert
        Assert.IsTrue(data.Count == 0);
    }

    [TestMethod]
    public void GetDocumentQueryResults_Id_Valid()
    {
        // Act
        var query1 = DocumentAnalysisUtilities.GetDocumentQueryResults(TextractData, "patientname").ToList();
        var query2 = DocumentAnalysisUtilities.GetDocumentQueryResults(TextractData, "dateofservice").ToList();

        //Assert
        Assert.IsTrue(query1.Count == 2);
        Assert.IsTrue(query2.Count == 2);
    }


    [TestMethod]
    public void GetDocumentQueryResults_Id_Inalid()
    {
        // Act
        var query1 = DocumentAnalysisUtilities.GetDocumentQueryResults(TextractData, "badquery").ToList();
        var query2 = DocumentAnalysisUtilities.GetDocumentQueryResults(TextractData, "invoicetotal").ToList();

        //Assert
        Assert.IsTrue(query1.Count == 0);
        Assert.IsTrue(query2.Count == 0);
    }
}
