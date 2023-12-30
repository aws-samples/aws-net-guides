namespace ServerlessDocProcessing;

public class DocAnalysisStackProps : StackProps
{
    public string EnvironmentName { get; init; }
    public string ResourceNamePrefix { get; init; }
    public string FunctionCodeBaseDirectory { get; init; }
}
