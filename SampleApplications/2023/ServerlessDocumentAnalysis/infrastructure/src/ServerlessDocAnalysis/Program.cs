using ServerlessDocProcessing;

var app = new App();
string environmentName = $"{app.Node.TryGetContext("environmentName")}";
string stackName = $"{app.Node.TryGetContext("stackName")}-{environmentName}";
string functionBaseDir = $"{app.Node.TryGetContext("functionBaseDirectory")}";
string resourcePrefix = $"{app.Node.TryGetContext("resourcePrefix")}";

_ = new DocAnalysisStack(app, "ServerlessDocProcessingStack", new DocAnalysisStackProps
{
    StackName = !string.IsNullOrEmpty(stackName) ? stackName : "docProcessingStack",
    EnvironmentName = !string.IsNullOrEmpty(environmentName) ? environmentName : "dev",
    FunctionCodeBaseDirectory = !string.IsNullOrEmpty(functionBaseDir) ? functionBaseDir : "./function-output",
    ResourceNamePrefix = !string.IsNullOrEmpty(resourcePrefix) ? resourcePrefix : "docprocessing"
});
app.Synth();