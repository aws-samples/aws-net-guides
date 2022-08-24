using Amazon.DAX;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace dydb.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly IDynamoDBContext _dynamoDBContext;
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _configuration;

        public WeatherForecastController(IDynamoDBContext dynamoDBContext, ILogger<WeatherForecastController> logger, IConfiguration configuration)
        {
            _dynamoDBContext = dynamoDBContext;
            _logger = logger;
            _configuration = configuration;
        }



        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get(string zipcode = "12345")
        {
            DaxClientConfig daxClientConfig = new DaxClientConfig(_configuration["DAXEndpoint"])
            {
                AwsCredentials = FallbackCredentialsFactory.GetCredentials()
            };
            var daxClient = new ClusterDaxClient(daxClientConfig);

            _logger.LogInformation("Call DB to get the data....Start....");
            List<WeatherForecast> items = new List<WeatherForecast>();

            var watch = System.Diagnostics.Stopwatch.StartNew();

            var request = new QueryRequest()
            {
                TableName = "WeatherForecast",
                KeyConditionExpression = "ZipCode = :zipcodeval",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":zipcodeval", new AttributeValue{ S = zipcode } }
                }
            };

            var response = daxClient.QueryAsync(request).Result;

            items.AddRange(response.Items.Select(item => new WeatherForecast()
            {
                Date = item["Date"].S,
                Summary = item["Summary"].S,
                TemperatureC = Int32.Parse(item["TemperatureC"].N),
                ZipCode = item["ZipCode"].S,
            }));

            watch.Stop();

            _logger.LogInformation("Call DB to get the data....End....");
            _logger.LogInformation($"Time Take to call DB to get the data....{watch.ElapsedMilliseconds} ms....");

            return items;
        }

        [HttpPost(Name = "GetWeatherForecast")]
        public async Task<string> Post(string zipcode)
        {

            var store = AddWeatherForecast(zipcode);
            foreach (var item in store)
            {
                await _dynamoDBContext.SaveAsync(item);
            }

            return "Success";
        }

        private static IEnumerable<WeatherForecast> AddWeatherForecast(string zipcode)
        {
            return Enumerable.Range(1, 500).Select(index => new WeatherForecast
            {
                ZipCode = zipcode,
                Date = DateTime.Now.AddDays(index).ToString(),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
                        .ToArray();
        }
    }
}