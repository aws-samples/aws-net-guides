using Amazon.Lambda.Annotations;
using DocProcessing.Shared;

namespace RestartStepFunction;

[LambdaStartup]
public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
    }
}