using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

namespace TMDBAlexa.Shared
{
    public class DynamoDBService
    {
        private readonly ILogger<DynamoDBService> _logger;
        private readonly AmazonDynamoDBClient _client = new AmazonDynamoDBClient();
        private readonly string _tableName = "movies";

        public DynamoDBService(ILogger<DynamoDBService> logger)
        {
            _logger = logger;
        }

        public async Task<MovieModel> SearchTable(string searchText)
        {
            DynamoDBContext context = new DynamoDBContext(_client);

            var conditions = new List<ScanCondition>()
            {
            new ScanCondition("TitleLower", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Contains, searchText.ToLower())
            };

            var queryResult = await context.ScanAsync<MovieModel>(conditions).GetRemainingAsync();

            if (queryResult.Count > 0)
            {
                return queryResult.FirstOrDefault();
            }
            return null;
        }

        public async Task CreateTable()
        {
            await DeleteTable();
            var request = new CreateTableRequest
            {
                AttributeDefinitions = new List<AttributeDefinition>()
            {
                new AttributeDefinition
                {
                    AttributeName = "id",
                    AttributeType = "N"
                },
                new AttributeDefinition
                {
                    AttributeName = "popularity",
                    AttributeType = "N"
                }
            },
                KeySchema = new List<KeySchemaElement>
            {
                new KeySchemaElement
                {
                    AttributeName = "id",
                    KeyType = "HASH" //Partition key
                },
                new KeySchemaElement
                {
                    AttributeName = "popularity",
                    KeyType = "RANGE" //Sort key
                }
            },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 6
                },
                TableName = _tableName
            };

            var response = await _client.CreateTableAsync(request);

            var tableDescription = response.TableDescription;
            _logger.LogInformation("{1}: {0} \t ReadsPerSec: {2} \t WritesPerSec: {3}",
                      tableDescription.TableStatus,
                      tableDescription.TableName,
                      tableDescription.ProvisionedThroughput.ReadCapacityUnits,
                      tableDescription.ProvisionedThroughput.WriteCapacityUnits);

            string status = tableDescription.TableStatus;
            _logger.LogInformation(_tableName + " - " + status);

            await WaitUntilTableReady(_tableName);
        }

        public async Task WriteToTable(SearchContainer<SearchMovie> results)
        {
            DynamoDBContext context = new DynamoDBContext(_client);

            BatchWrite<MovieModel> model = context.CreateBatchWrite<MovieModel>();

            foreach (var movie in results.Results)
            {
                model.AddPutItem(new MovieModel
                {
                    Id = movie.Id,
                    Overview = movie.Overview,
                    Popularity = movie.Popularity,
                    ReleaseDate = movie.ReleaseDate.ToString(),
                    Title = movie.Title,
                    TitleLower = movie.Title.ToLower(),
                    VoteAverage = movie.VoteAverage,
                    VoteCount = movie.VoteCount
                });
            }

            await model.ExecuteAsync();
        }

        private async Task DeleteTable()
        {
            try
            {
                await _client.DescribeTableAsync(new DescribeTableRequest
                {
                    TableName = _tableName
                });

                DeleteTableRequest deleteRequest = new DeleteTableRequest
                {
                    TableName = _tableName
                };

                await _client.DeleteTableAsync(deleteRequest);

                Thread.Sleep(5000);
            }
            catch (ResourceNotFoundException)
            { }
        }

        private async Task WaitUntilTableReady(string tableName)
        {
            string status = null;
            // Let us wait until table is created. Call DescribeTable.
            do
            {
                System.Threading.Thread.Sleep(5000); // Wait 5 seconds.
                try
                {
                    var res = await _client.DescribeTableAsync(new DescribeTableRequest
                    {
                        TableName = tableName
                    });

                    _logger.LogInformation("Table name: {0}, status: {1}",
                              res.Table.TableName,
                              res.Table.TableStatus);
                    status = res.Table.TableStatus;
                }
                catch (ResourceNotFoundException)
                {
                    // DescribeTable is eventually consistent. So you might
                    // get resource not found. So we handle the potential exception.
                }
            } while (status != "ACTIVE");
        }
    }
}
