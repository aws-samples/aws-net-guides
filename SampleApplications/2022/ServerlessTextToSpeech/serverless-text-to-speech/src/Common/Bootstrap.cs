using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Polly;
using Amazon.S3;
using Amazon.StepFunctions;
using Amazon.Textract;
using Microsoft.Extensions.DependencyInjection;
using Common.Services;
using System.Diagnostics.CodeAnalysis;

namespace Common;

public static class Bootstrap
{
    private static ServiceCollection Services { get; }

    [NotNull]
    public static ServiceProvider? ServiceProvider { get; private set; }


    static Bootstrap()
    {
        Services = new ServiceCollection();
    }

    public static void ConfigureServices()
    {
        Services.AddDefaultAWSOptions(new()
        {
            Region = RegionEndpoint.GetBySystemName(Environment.GetEnvironmentVariable("AWS_REGION"))
        });

        Services.AddAWSService<IAmazonPolly>();
        Services.AddAWSService<IAmazonTextract>();
        Services.AddAWSService<IAmazonStepFunctions>();
        Services.AddAWSService<IAmazonS3>();

        // DynamoDB and object model
        Services.AddAWSService<IAmazonDynamoDB>();
        Services.AddTransient<IDynamoDBContext>(c => new
            DynamoDBContext(c.GetService<IAmazonDynamoDB>(),
                new DynamoDBContextConfig
                {
                    TableNamePrefix = $"{Environment.GetEnvironmentVariable("STAGE_NAME")}-"
                }));

        // Utilities and internal classes
        Services.AddSingleton<ITextToSpeechUtilities, TextToSpeechUtilities>();

        ServiceProvider = Services.BuildServiceProvider();
    }
}
