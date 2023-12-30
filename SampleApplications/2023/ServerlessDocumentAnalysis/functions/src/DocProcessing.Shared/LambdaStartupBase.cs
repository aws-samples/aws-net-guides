using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.S3;
using Amazon.StepFunctions;
using Amazon.Textract;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using DocProcessing.Shared.AwsSdkUtilities;
using DocProcessing.Shared.Service;
using Microsoft.Extensions.DependencyInjection;

namespace DocProcessing.Shared;

public abstract class StartupBase
{
    public virtual void ConfigureServices(IServiceCollection services)
    {
        AWSSDKHandler.RegisterXRayForAllServices();
        services.AddTransient<IDataService, DataService>()
            .AddTransient<ITextractService, TextractService>()
            .AddAWSService<IAmazonS3>()
            .AddAWSService<IAmazonTextract>()
            .AddAWSService<IAmazonStepFunctions>()
            .AddAWSService<IAmazonDynamoDB>()
            .AddTransient<IDynamoDBContext>(c => new
                DynamoDBContext(c.GetService<IAmazonDynamoDB>(),
                    new DynamoDBContextConfig
                    {
                        TableNamePrefix = $"{Environment.GetEnvironmentVariable("ENVIRONMENT_NAME")}-"
                    }))
            .BuildServiceProvider();
    }
}