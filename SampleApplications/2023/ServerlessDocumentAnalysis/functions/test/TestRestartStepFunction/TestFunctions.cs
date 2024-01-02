using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using Amazon.StepFunctions;
using Amazon.StepFunctions.Model;
using DocProcessing.Shared.Model.Data;
using DocProcessing.Shared.Service;
using Moq;
using RestartStepFunction;
using RestartStepFunction.Exceptions;
using RestartStepFunction.Model;
using System.Text.Json;

namespace TestRestartStepFunction;

[TestClass]
public class TestFunctions
{
    readonly Mock<ILambdaContext> _context = new();
    Mock<IAmazonStepFunctions> _stepFunctionClient;
    Mock<IDataService> _mockDataService;

    [TestInitialize]
    public void Setup()
    {
        // Mock the Step Function Client
        _stepFunctionClient = new Mock<IAmazonStepFunctions>();
        _stepFunctionClient.Setup(a => a.SendTaskSuccessAsync(It.IsAny<SendTaskSuccessRequest>(), new CancellationToken()).Result)
             .Returns(new SendTaskSuccessResponse
             {
                 HttpStatusCode = System.Net.HttpStatusCode.OK
             });

        _stepFunctionClient.Setup(a => a.SendTaskFailureAsync(It.IsAny<SendTaskFailureRequest>(), new CancellationToken()).Result)
             .Returns(new SendTaskFailureResponse
             {
                 HttpStatusCode = System.Net.HttpStatusCode.BadRequest
             });

        // Mock the data service
        _mockDataService = new Mock<IDataService>();
        _mockDataService.Setup(a => a.GetData<ProcessData>(It.Is<string>(a => a == "notasktoken")).Result)
            .Returns(new ProcessData
            {
                TextractTaskToken = null
            });
        _mockDataService.Setup(a => a.GetData<ProcessData>(It.Is<string>(a => a == "withasktoken")).Result)
            .Returns(new ProcessData
            {
                TextractTaskToken = "tasktoken"
            });
    }

    [TestMethod]
    public async Task RestartStepFunction_NoRecord_Exception()
    {
        //Arrange
        SNSEvent evt = new()
        {
            Records = []
        };

        var testHandler = new Function(_stepFunctionClient.Object, _mockDataService.Object);

        //Act / Assert
        var exc = await Assert.ThrowsExceptionAsync<RestartStepFunctionException>(() => testHandler.FunctionHandler(evt, _context.Object));
        Assert.IsTrue(exc.Message == "No message received");
    }

    public async Task RestartStepFunction_MissingTaskToken_Exception()
    {

        // Arrange
        TextractCompletionModel model = new()
        {
            JobTag = "notasktoken",
            Status = TextractCompletionModel.SUCCESS_STATUS
        };

        SNSEvent evt = new()
        {
            Records = new List<SNSEvent.SNSRecord>
            {
                new() {
                    Sns = new SNSEvent.SNSMessage
                    {
                        Message = JsonSerializer.Serialize(model)
                    }
                }
            }
        };

        var testHandler = new Function(_stepFunctionClient.Object, _mockDataService.Object);

        //Act / Assert
        var exc = await Assert.ThrowsExceptionAsync<RestartStepFunctionException>(() => testHandler.FunctionHandler(evt, _context.Object));
        Assert.IsTrue(exc.Message == "Missing Task Token");
    }
}










