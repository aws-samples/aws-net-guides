# aws-sample-dydb-api

## Summary

Sample Web API uses DynamoDB as backend Database and leverage DynamoDB Accelerator for the Read.


## Architecture

![Architecture](images\architecture.jpg)

## Prerequisites

- [Create an AWS account](https://portal.aws.amazon.com/gp/aws/developer/registration/index.html) if you do not already have one and log in. The IAM user that you use must have sufficient permissions to make necessary AWS service calls and manage AWS resources.
- [Visual Studio Community Edition](https://visualstudio.microsoft.com/vs/)

## Create API application in Visual Studio

1. Create New WebApi App in Visual Studio

2. Add the Nuget Package

    ```powershell
    dotnet add package AWSSDK.DynamoDBv2
    dotnet add package AWSSDK.DAX.Client
    ```

3. Update the Program.cs file for DynamoDB

    ```C#
    var credentials = FallbackCredentialsFactory.GetCredentials();
    var config = new AmazonDynamoDBConfig()
    {
        RegionEndpoint = Amazon.RegionEndpoint.USEast1
    };
    var client = new AmazonDynamoDBClient(credentials, config);
    builder.Services.AddSingleton<IAmazonDynamoDB>(client);
    builder.Services.AddSingleton<IDynamoDBContext, DynamoDBContext>();
    
    ```

4. Update WeatherForecast.cs

    ```C#
    public class WeatherForecast
        {
            public string ZipCode { get; set; }
            public string Date { get; set; }
            public int TemperatureC { get; set; }
            public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
            public string? Summary { get; set; }
        }
    ```

    > Makesure the Model Name and Table Name exact match and Properties and Table Attributes are exact match.


5. Add Put in the Controller - WeatherForecastController.cs to write to DynamoDB

    ```C#

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

    ```

6. Update the Get Method - WeatherForecastController.cs to use DAX

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

7. Update the DAX endpoint in the AppConfig file.

## Additional Requirement for validation

- Make sure Security Group allow connectivity from EC2/App Subnet to the DAX Subnet 
- IAM role has the necessary permissions
