using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMDBAlexa.Shared;

namespace TMDBAlexa.SeedData
{
    public class Startup
    {
        public IServiceProvider Setup()
        {
            var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

            var services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(configuration);

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();                
            });


            ConfigureServices(configuration, services);

            IServiceProvider provider = services.BuildServiceProvider();

            return provider;
        }

        private void ConfigureServices(IConfiguration configuration, ServiceCollection services)
        {
            AWSOptions awsOptions = configuration.GetAWSOptions();
            services.AddDefaultAWSOptions(awsOptions);
            services.AddAWSService<IAmazonDynamoDB>();
            
            services.AddSingleton<LambdaEntryPoint, LambdaEntryPoint>();
            services.AddSingleton<DynamoDBService, DynamoDBService>();
        }
    }
}
