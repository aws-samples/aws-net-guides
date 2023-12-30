using Amazon.Lambda.Annotations;
using DocProcessing.Shared;

namespace SubmitToTextract;

[LambdaStartup]
public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
    }
}