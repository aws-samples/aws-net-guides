# aws-sample-dydb-api

## Updates after the DynamoDB implimentation (main branbch code)

- Add the Nuget Package

```powershell
dotnet add package AWSSDK.DAX.Client
```

- Update the Get in [WeatherForecastController.cs](dydb.api/Controllers/WeatherForecastController.cs )

```C#
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

```
