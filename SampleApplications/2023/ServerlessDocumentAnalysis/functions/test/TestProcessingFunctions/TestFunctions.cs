using Amazon.Lambda.Core;
using DocProcessing.Shared.Exceptions;
using DocProcessing.Shared.Model;
using DocProcessing.Shared.Model.Data;
using DocProcessing.Shared.Service;
using InitializeProcessing;
using InitializeProcessing.Input;
using ProcessingFunctions.Input;

namespace TestProcessingFunctions;

[TestClass]
public class TestFunctions
{
    readonly Mock<ILambdaContext> _context = new();

    [TestInitialize]
    public void Setup()
    {
        Environment.SetEnvironmentVariable("ALLOWED_FILE_EXTENSIONS", ".pdf");
    }

    [TestMethod]
    public async Task SuccessOutputHandler_Success_Message()
    {
        //Arrange - this 
        var dataMock = new Mock<IDataService>();
        dataMock.Setup(p => p.GetData<ProcessData>(It.IsAny<string>()).Result)
            .Returns(new ProcessData
            {
                ExternalId = "ext",
                ExecutionId = "exc",
                InputDocKey = "inpkey",
                InputDocBucket = "inpbucket",
                ExpenseReports = [],
                Queries = []
            });

        var testHandler = new Function(dataMock.Object);

        //Act (run the lamdba)
        var output = await testHandler.SuccessOutputHandler(new IdMessage { Id = "testId" }, _context.Object);

        //Assert that thevalues pass through properly
        Assert.IsTrue(output.ExternalId == "ext");
        Assert.IsTrue(output.Execution == "exc");
        Assert.IsTrue(output.InputKey == "inpkey");
        Assert.IsTrue(output.InputBucket == "inpbucket");
        Assert.IsTrue(output.ExpenseReports.Count == 0);
        Assert.IsTrue(output.Queries.Count == 0);
    }

    [TestMethod]
    public async Task FailOutputHandler_Fail_Message()
    {
        //Arrange
        var dataMock = new Mock<IDataService>();
        dataMock.Setup(p => p.GetBySingleIndex<ProcessData>(It.IsAny<string>(), It.Is<string>(a => a == "executionIndex")).Result)
            .Returns([new ProcessData()
            {
                ExternalId = "ext",
                InputDocKey = "key",
                InputDocBucket = "bucket"
            }]);

        ErrorInput error = new()
        {
            Cause = "Unit Test",
            Execution = "exc",
            Error = "Exception thrown"
        };

        var testHandler = new Function(dataMock.Object);

        //Act
        var output = await testHandler.FailOutputHandler(error, _context.Object);

        //Assert
        Assert.IsTrue(output.Execution == "exc");
        Assert.IsTrue(output.ExternalId == "ext");
        Assert.IsTrue(output.InputKey == "key");
        Assert.IsTrue(output.InputBucket == "bucket");
        Assert.IsTrue(output.Error.Error == "Exception thrown");
        Assert.IsTrue(output.Error.Cause == "Unit Test");
    }

    [TestMethod]
    public async Task InitializeHandler_Valid_Message()
    {
        //Arrange
        var eventMock = new Mock<S3StepFunctionCompositeEvent>();
        eventMock.As<IProcessDataInitializer>().Setup(a => a.ExecutionId).Returns("exc");
        eventMock.As<IProcessDataInitializer>().Setup(a => a.BucketName).Returns("bucket");
        eventMock.As<IProcessDataInitializer>().Setup(a => a.Key).Returns("key");
        eventMock.As<IProcessDataInitializer>().Setup(a => a.FileExtension).Returns(".pdf");

        var dataMock = new Mock<IDataService>();
        dataMock.Setup(p => p.InitializeProcessData(It.IsAny<IProcessDataInitializer>(), It.Is<string>(a => a == "Id"), It.Is<string>(a => a == "Queries")).Result)
            .Returns(new ProcessData()
            {
                Id = "id",
                FileExtension = eventMock.As<IProcessDataInitializer>().Object.FileExtension
            });

        var testHandler = new Function(dataMock.Object);

        //Act
        var output = await testHandler.InitializeHandler(eventMock.Object, _context.Object);

        //Assert
        Assert.IsTrue(output.Id == "id");


    }


    [TestMethod]
    public async Task InitializeHandler_BadFileType_Exception()
    {
        //Arrange
        var eventMock = new Mock<S3StepFunctionCompositeEvent>();
        eventMock.As<IProcessDataInitializer>().Setup(a => a.ExecutionId).Returns("exc");
        eventMock.As<IProcessDataInitializer>().Setup(a => a.BucketName).Returns("bucket");
        eventMock.As<IProcessDataInitializer>().Setup(a => a.Key).Returns("key");
        eventMock.As<IProcessDataInitializer>().Setup(a => a.FileExtension).Returns(".doc");

        var dataMock = new Mock<IDataService>();
        dataMock.Setup(p => p.InitializeProcessData(It.IsAny<IProcessDataInitializer>(), It.Is<string>(a => a == "Id"), It.Is<string>(a => a == "Queries")).Result)
            .Returns(new ProcessData()
            {
                Id = "id",
                FileExtension = eventMock.As<IProcessDataInitializer>().Object.FileExtension
            });

        var testHandler = new Function(dataMock.Object);

        //Act / Assert
        var exc = await Assert.ThrowsExceptionAsync<FileTypeException>(async () => await testHandler.InitializeHandler(eventMock.Object, _context.Object));
        Assert.IsTrue(exc.Id == "id");
    }
}