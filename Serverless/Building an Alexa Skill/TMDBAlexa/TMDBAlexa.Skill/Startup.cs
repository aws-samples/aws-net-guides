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

namespace TMDBAlexa.Skill
{
    public class Startup
    {
        public IServiceProvider Setup()
        {
            var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())  
                    .AddEnvironmentVariables() 
                    .Build();

            var services = new ServiceCollection();

            // Add configuration service
            services.AddSingleton<IConfiguration>(configuration);

            // Add logging service
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
            #region AWS SDK setup
            // Get the AWS profile information from configuration providers
            AWSOptions awsOptions = configuration.GetAWSOptions();

            // Configure AWS service clients to use these credentials
            services.AddDefaultAWSOptions(awsOptions);

            // These AWS service clients will be singleton by default
            services.AddAWSService<IAmazonDynamoDB>();
            #endregion

            services.AddSingleton<LambdaEntryPoint, LambdaEntryPoint>();
            services.AddSingleton<DynamoDBService, DynamoDBService>();
        }
    }
}
