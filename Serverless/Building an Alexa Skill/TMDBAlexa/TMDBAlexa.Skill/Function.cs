using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using TMDbLib.Objects.Movies;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace TMDBAlexa.Skill;

public class Function
{
    private readonly LambdaEntryPoint _entryPoint;

    public Function()
    {
        var startup = new Startup();
        IServiceProvider provider = startup.Setup();

        _entryPoint = provider.GetRequiredService<LambdaEntryPoint>();
    }

    public async Task<SkillResponse> FunctionHandler(SkillRequest input, ILambdaContext context)
    {
        ILambdaLogger log = context.Logger;
        log.LogLine($"Skill Request Object:" + JsonConvert.SerializeObject(input));
        return await _entryPoint.Handler(input);
    }
}
