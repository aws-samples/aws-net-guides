using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.SNS.Subscriptions;

namespace ServerlessDocProcessing.Lambda;

public class FunctionCollectionConstruct : Construct
{
    private string EnvironmentName { get; }

    public Function InitializeFunction { get; init; }
    public Function SuccessFunction { get; init; }
    public Function FailureFunction { get; init; }
    public Function SubmitToTextractFunction { get; init; }
    public Function RestartStepFunction { get; init; }
    public Function SubmitToTextractExpenseFunction { get; init; }
    public Function ProcessTextractQueryResultFunction { get; init; }
    public Function ProcessTextractExpenseResultFunction { get; init; }

    public FunctionCollectionConstruct(Construct scope, string id, FunctionCollectionProps props)
        : base(scope, id)
    {
        EnvironmentName = props.EnvironmentName;

        // Set up the function properties
        CustomFunctionProps.EnvironmentName = props.EnvironmentName;
        CustomFunctionProps.FunctionBaseDirectory = props.FunctionCodeBaseDirectory;
        CustomFunctionProps.AddGlobalEnvironment("POWERTOOLS_SERVICE_NAME", $"docprocessing-{EnvironmentName}");
        CustomFunctionProps.AddGlobalEnvironment("POWERTOOLS_LOG_LEVEL", $"Debug");
        CustomFunctionProps.AddGlobalEnvironment("POWERTOOLS_LOGGER_CASE", $"SnakeCase");
        CustomFunctionProps.AddGlobalEnvironment("POWERTOOLS_LOGGER_LOG_EVENT", $"true");
        CustomFunctionProps.AddGlobalEnvironment("POWERTOOLS_LOGGER_SAMPLE_RATE", $"0");
        CustomFunctionProps.AddGlobalEnvironment("POWERTOOLS_TRACE_DISABLED", $"false");
        CustomFunctionProps.AddGlobalEnvironment("POWERTOOLS_TRACER_CAPTURE_RESPONSE", $"true");
        CustomFunctionProps.AddGlobalEnvironment("POWERTOOLS_TRACER_CAPTURE_ERROR", $"true");
        CustomFunctionProps.AddGlobalEnvironment("POWERTOOLS_METRICS_NAMESPACE", $"SubmitToTextract-{EnvironmentName}");
        CustomFunctionProps.AddGlobalEnvironment("ALLOWED_FILE_EXTENSIONS", $".pdf");
        CustomFunctionProps.AddGlobalEnvironment("ENVIRONMENT_NAME", EnvironmentName);

        // Function to initialize the process. It will create the relevant data structures etc.
        InitializeFunction = new CustomFunction(this, "InitializeProcessing", new CustomFunctionProps
        {
            FunctionNameBase = "InitializeProcessing",
            Description = "Initializes the document processing",
            FunctionCodeDirectory = "ProcessingFunctions",
            ResourcePrefix = props.ResourceNamePrefix
        }).AddAnnotationsHandler("InitializeHandler");

        // Grant access to the main data table for Object Persistence Model
        props.ProcessDataTable.GrantReadWriteData(InitializeFunction);

        // Grant access to the query config data table for Object Persistence Model
        props.QueryConfigTable.GrantReadWriteData(InitializeFunction);

        // Allow the function read from the input bucket. This allows the initialization function
        // to be able get metadata about the document that is triggering it
        props.InputBucket.GrantReadWrite(InitializeFunction);

        // Function that sends a specially formatted message to the SQS queue for both success and failure
        SuccessFunction = new CustomFunction(this, "SuccessFunction", new CustomFunctionProps
        {
            FunctionNameBase = "SuccessFunction",
            FunctionCodeDirectory = "ProcessingFunctions",
            Description = "Sends a success message to the SQS queue",
            ResourcePrefix = props.ResourceNamePrefix
        }).AddAnnotationsHandler("SuccessOutputHandler");

        // Grant access to the main data table for Object Persistence Model
        props.ProcessDataTable.GrantReadWriteData(SuccessFunction);

        // Function that sends a specially formatted message to the SQS queue for failure
        FailureFunction = new CustomFunction(this, "FailFunction", new CustomFunctionProps
        {
            FunctionNameBase = "FailFunction",
            FunctionCodeDirectory = "ProcessingFunctions",
            Description = "Sends a faliure message to the SQS queue",
            ResourcePrefix = props.ResourceNamePrefix
        }).AddAnnotationsHandler("FailOutputHandler");

        // Grant access to the main data table for Object Persistence Model
        props.ProcessDataTable.GrantReadWriteData(FailureFunction);

        // Function that submits the document to the textract service
        SubmitToTextractFunction = new CustomFunction(this, "SubmitToTextract", new CustomFunctionProps
        {
            FunctionNameBase = "SubmitToTextract",
            FunctionCodeDirectory = "SubmitToTextract",
            Description = "Submits the document to Textract for analysis",
            ResourcePrefix = props.ResourceNamePrefix
        }).AddAnnotationsHandler("SubmitToTextractForStandardAnalysis");

        // Grant access to the main data table for Object Persistence Model
        props.ProcessDataTable.GrantReadWriteData(SubmitToTextractFunction);

        // Supply appropriate environment variables to the function that submits to Textract
        SubmitToTextractFunction
            .AddEnvironment("TEXTRACT_BUCKET", props.TextractBucket.BucketName)
            .AddEnvironment("TEXTRACT_TOPIC", props.TextractTopic.TopicArn)
            .AddEnvironment("TEXTRACT_OUTPUT_KEY", "textract-output")
            .AddEnvironment("TEXTRACT_ROLE", props.TextractRole.RoleArn);

        //Function needs Textract Permissions. Grant here.
        SubmitToTextractFunction.Role.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonTextractFullAccess"));

        // Allows the function to retrieve the document from S3
        SubmitToTextractFunction.AddToRolePolicy(AllowInputBucket(props.InputBucket));
        SubmitToTextractFunction.AddToRolePolicy(AllowTextractBucket(props.TextractBucket));

        // Function that submits the document to the textract service for Expense Analysis
        SubmitToTextractExpenseFunction = new CustomFunction(this, "SubmitToTextractExpense", new CustomFunctionProps
        {
            FunctionNameBase = "SubmitToTextractExpense",
            FunctionCodeDirectory = "SubmitToTextract",
            Description = "Submits the document to Textract for expense analysis",
            ResourcePrefix = props.ResourceNamePrefix
        }).AddAnnotationsHandler("SubmitToTextractForExpenseAnalysis");

        // Grant access to the main data table for Object Persistence Model
        props.ProcessDataTable.GrantReadWriteData(SubmitToTextractExpenseFunction);

        // Supply appropriate environment variables to the function that submits to Textract for expense analysis
        SubmitToTextractExpenseFunction
            .AddEnvironment("TEXTRACT_BUCKET", props.TextractBucket.BucketName)
            .AddEnvironment("TEXTRACT_TOPIC", props.TextractTopic.TopicArn)
            .AddEnvironment("TEXTRACT_EXPENSE_OUTPUT_KEY", "expense-output")
            .AddEnvironment("TEXTRACT_ROLE", props.TextractRole.RoleArn);

        //Function needs Textract Permissions. Grant here.
        SubmitToTextractExpenseFunction.Role.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonTextractFullAccess"));

        // Allows the function to retrieve the document from S3
        SubmitToTextractExpenseFunction.AddToRolePolicy(AllowInputBucket(props.InputBucket));
        SubmitToTextractExpenseFunction.AddToRolePolicy(AllowTextractBucket(props.TextractBucket));

        // Function that process the textract data and outputs results to DynamoDB
        ProcessTextractQueryResultFunction = new CustomFunction(this, "ProcessTextractQueryResults", new CustomFunctionProps
        {
            FunctionNameBase = "ProcessTextractQueryResults",
            Description = "Processes the general Textract results and extracts query information",
            ResourcePrefix = props.ResourceNamePrefix
        });

        // Grant access to the main data table for Object Persistence Model
        props.ProcessDataTable.GrantReadWriteData(ProcessTextractQueryResultFunction);

        // The query processing function needs to be able to read from the S3 bucket.
        props.TextractBucket.GrantRead(ProcessTextractQueryResultFunction);

        // Function that process the textract Expense data and outputs results to DynamoDB
        ProcessTextractExpenseResultFunction = new CustomFunction(this, "ProcessTextractExpenseResults", new CustomFunctionProps
        {
            FunctionNameBase = "ProcessTextractExpenseResults",
            Description = "Processes textract expense results",
            ResourcePrefix = props.ResourceNamePrefix
        });

        // Grant access to the main data table for Object Persistence Model
        props.ProcessDataTable.GrantReadWriteData(ProcessTextractExpenseResultFunction);

        // The Expense processing function needs to be able to read from the S3 bucket.
        props.TextractBucket.GrantRead(ProcessTextractExpenseResultFunction);


        // Function that restarts the step function following the aynchronous completion of Textract
        RestartStepFunction = new CustomFunction(this, "RestartStepFunction", new CustomFunctionProps
        {
            FunctionNameBase = "RestartStepFunction",
            Description = "Restarts the step function after Textract Processing is complete",
            ResourcePrefix = props.ResourceNamePrefix
        });

        // Grant access to the main data table for Object Persistence Model
        props.ProcessDataTable.GrantReadWriteData(RestartStepFunction);

        // Add a subscription to the lambda function that will be restarting the step function, to the topic that Textract published 
        props.TextractTopic.AddSubscription(new LambdaSubscription(RestartStepFunction));

        // The whole purpose of this function is to restart step functions, so it will need basic permissions for that
        // Allows the function to restart the step function
        RestartStepFunction.AddToRolePolicy(new PolicyStatement(new PolicyStatementProps
        {
            Effect = Effect.ALLOW,
            Actions = new[] { "states:SendTaskSuccess", "states:SendTaskFailure" },
            Resources = new[] { "*" }
        }));
    }

    // Helper method to give functions access to the input bucket
    private static PolicyStatement AllowInputBucket(IBucket bucket) => new(new PolicyStatementProps
    {
        Actions = new[] { "s3:Get*" },
        Resources = new[]
        {
            bucket.BucketArn,
            bucket.ArnForObjects("*")
        },
        Effect = Effect.ALLOW
    });

    // Policy that allows a function full access to the textract bucket ARN (Needed for enabling textract to write out results to S3
    private static PolicyStatement AllowTextractBucket(IBucket bucket) => new(new PolicyStatementProps
    {
        Actions = new[] { "s3:Put*", "s3:Get*" },
        Resources = new[]
        {
            bucket.BucketArn,
            bucket.ArnForObjects("*")
        },
        Effect = Effect.ALLOW
    });
}
