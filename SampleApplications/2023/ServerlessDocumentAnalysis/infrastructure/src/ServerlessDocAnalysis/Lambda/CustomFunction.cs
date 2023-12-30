using Amazon.CDK.AWS.IAM;

namespace ServerlessDocProcessing.Lambda;

public class CustomFunction : Function
{
    public CustomFunction(Construct scope, string id, CustomFunctionProps props)
        : base(scope, id, props.FunctionProps)
    {

        var cfnFcn = (CfnFunction)Node.DefaultChild;
        // For Future Use with SAM
        cfnFcn.AddMetadata("BuildMethod", props.BuildMethod);
        cfnFcn.OverrideLogicalId(props.FunctionName);

        // Add Global Environment Variables
        foreach (var env in CustomFunctionProps.GlobalEnvironment)
        {
            AddEnvironment(env.Key, env.Value);
        }

        // Allow metrics to be added by this function
        AddToRolePolicy(new PolicyStatement(new PolicyStatementProps
        {
            Actions = new[] { "cloudwatch:PutMetricData" },
            Effect = Effect.ALLOW,
            Resources = new[] { "*" }
        }));
    }

    public CustomFunction AddAnnotationsHandler(string handlername)
    {
        AddEnvironment("ANNOTATIONS_HANDLER", handlername);
        return this;
    }

    public CustomFunction AddEnvironmentVariable(string key, string value)
    {
        AddEnvironment(key, value);
        return this;
    }

}