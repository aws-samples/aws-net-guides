using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMDbLib.Client;
using Microsoft.Extensions.Configuration;
using TMDBAlexa.Shared;

namespace TMDBAlexa.SeedData
{
    public class LambdaEntryPoint
    {
        private readonly ILogger<LambdaEntryPoint> _logger;
        private readonly DynamoDBService _dbService;
        private readonly IConfiguration _config;
        public LambdaEntryPoint(ILogger<LambdaEntryPoint> logger, DynamoDBService dbService, IConfiguration config)
        {
            _logger = logger;
            _dbService = dbService;
           _config = config;
        }

        public async Task<string> Handler()
        {
            _logger.LogInformation("Handler invoked");

            TMDbClient client = new TMDbClient(_config["TMDBApiKey"]);

            await _dbService.CreateTable();

            for (int i = 1; i < 100; i++)
            {
                var results = await client.GetMoviePopularListAsync("en-US", i);

                await _dbService.WriteToTable(results);
            }

            return "Done";
        }
    }
}
