using Amazon.CDK.AWS.Events;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.SQS;

namespace ServerlessDocProcessing.StepFunctions;

public class StepFunctionProps
{
    public string EnvironmentName { get; init; }
    public string ResourceNamePrefix { get; init; }
    public IFunction InitializeFunction { get; init; }
    public IFunction SuccessFunction { get; set; }
    public IFunction FailureFunction { get; set; }
    public IFunction SubmitToTextractFunction { get; init; }
    public IFunction SubmitToTextractExpenseFunction { get; init; }
    public IFunction ProcessTextractQueryFunction { get; init; }
    public IFunction ProcessTextractExpenseFunction { get; set; }
    public IQueue SendFailureQueue { get; init; }
    public IQueue SendSuccessQueue { get; init; }
    public IQueue DeadLetterQueue { get; init; }
    public IRole EventBridgeRole { get; init; }
    public Rule EventBridgeRule { get; init; }
}
