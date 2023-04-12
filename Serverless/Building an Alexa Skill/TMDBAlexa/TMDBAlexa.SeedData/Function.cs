using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace TMDBAlexa.SeedData;

public class Function
{
    private readonly LambdaEntryPoint _entryPoint;

    public Function()
    {
        var startup = new Startup();
        IServiceProvider provider = startup.Setup();

        _entryPoint = provider.GetRequiredService<LambdaEntryPoint>();
    }

    public async Task<string> FunctionHandler(ILambdaContext context)
    {
        return await _entryPoint.Handler();
    }
}
