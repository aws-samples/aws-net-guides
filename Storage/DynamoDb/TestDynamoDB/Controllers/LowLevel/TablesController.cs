using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Microsoft.AspNetCore.Mvc;
using TestDynamoDB.Utilities;

namespace TestDynamoDB.Controllers.LowLevel
{
    [Route("api/lowlevel/[controller]")]
    public class TablesController : Controller
    {
        private IAmazonDynamoDB _dynamoClient;
        private DynamoDBContextBuilder _opmDynamoContextBuilder;
        public TablesController(IAmazonDynamoDB dynamoClient, DynamoDBContextBuilder opmDynamoContextBuilder)
        {
            _dynamoClient = dynamoClient;
            _opmDynamoContextBuilder = opmDynamoContextBuilder;
        }

        [HttpGet()]
        public async Task<IActionResult> GetTables()
        {
            var req = new ListTablesRequest();
            req.Limit = 100;
            await _opmDynamoContextBuilder.Build();
            var res = await _dynamoClient.ListTablesAsync(req);
            foreach (var item in res.TableNames)
            {
                Console.WriteLine("Table Name: {0}", item);
            }
            return Ok(res.TableNames);
        }
        [HttpGet("{tableName}")]
        public async Task<IActionResult> Get(string tableName)
        {
            Console.WriteLine("*** Retrieving table information ***");
            Console.WriteLine($"Searching for table: {tableName}");

            DescribeTableRequest req = new DescribeTableRequest();
            req.TableName = tableName;
            try
            {
                var res = await _dynamoClient.DescribeTableAsync(req);
                var description = res.Table;
                Console.WriteLine($"Name: {description.TableName}");
                Console.WriteLine($"# of items: {description.ItemCount}");
                Console.WriteLine($"Provision Throughput (reads/sec): {description.ProvisionedThroughput.ReadCapacityUnits}");
                Console.WriteLine($"Provision Throughput (writes/sec): {description.ProvisionedThroughput.WriteCapacityUnits}");
                return Ok(res.Table);
            }
            catch (AmazonDynamoDBException addbe)
            {
                return AmazonExceptionHandlers.HandleAmazonDynamoDBException(addbe);
            }
            catch (AmazonServiceException ase)
            {
                AmazonExceptionHandlers.HandleAmazonServiceExceptionException(ase);
            }
            catch (AmazonClientException ace)
            {
                AmazonExceptionHandlers.HandleAmazonClientExceptionException(ace);
            }
            return StatusCode(500);
        }
        [HttpGet("{tableName}/status")]
        public async Task<IActionResult> GetTableStatus(string tableName)
        {
            Console.WriteLine("*** Retrieving table status information ***");
            Console.WriteLine($"Searching for table status: {tableName}");

            DescribeTableRequest req = new DescribeTableRequest();
            req.TableName = tableName;
            try
            {
                var res = await _dynamoClient.DescribeTableAsync(req);
                var description = res.Table.TableStatus;
                return Ok(description);
            }
            catch (AmazonDynamoDBException addbe)
            {
                return AmazonExceptionHandlers.HandleAmazonDynamoDBException(addbe);
            }
            catch (AmazonServiceException ase)
            {
                AmazonExceptionHandlers.HandleAmazonServiceExceptionException(ase);
            }
            catch (AmazonClientException ace)
            {
                AmazonExceptionHandlers.HandleAmazonClientExceptionException(ace);
            }
            return StatusCode(500);
        }
        [HttpPost()]
        public async Task<IActionResult> Create([FromBody] CreateTableRequest request)
        {
            Console.WriteLine("Request Info:");
            Console.WriteLine("\tTableName:");
            Console.WriteLine($"\t{request.TableName}");
            if (request.ProvisionedThroughput != null)
            {
                Console.WriteLine("\tReadCapacityUnits:");
                Console.WriteLine($"\t{request.ProvisionedThroughput.ReadCapacityUnits}");
                Console.WriteLine("\tWriteCapacityUnits:");
                Console.WriteLine($"\t{request.ProvisionedThroughput.WriteCapacityUnits}");
            }
            if (request.KeySchema != null)
            {
                System.Console.WriteLine("\tKeySchemas:");
                foreach (var k in request.KeySchema)
                {
                    Console.WriteLine("\tAttributeName");
                    Console.WriteLine($"\t{k.AttributeName}");
                    Console.WriteLine("\tKeyType");
                    Console.WriteLine($"\t{k.KeyType}");
                }
            }
            try
            {
                var res = await _dynamoClient.CreateTableAsync(request);
                Console.WriteLine("Created table");
                System.Console.WriteLine("\tTableName");
                System.Console.WriteLine($"\t{res.TableDescription.TableName}");
                System.Console.WriteLine("\tTableArn");
                System.Console.WriteLine($"\t{res.TableDescription.TableArn}");
                System.Console.WriteLine("\tTableId");
                System.Console.WriteLine($"\t{res.TableDescription.TableId}");
                System.Console.WriteLine("\tTableStatus");
                System.Console.WriteLine($"\t{res.TableDescription.TableStatus}");
                return new JsonResult(
                    new
                    {
                        message = $"Creating new table: {res.TableDescription.TableName}",
                        TableDescription = res.TableDescription
                    })
                { StatusCode = 202 };
            }
            catch (AmazonDynamoDBException addbe)
            {
                return AmazonExceptionHandlers.HandleAmazonDynamoDBException(addbe);
            }
            catch (AmazonServiceException ase)
            {
                AmazonExceptionHandlers.HandleAmazonServiceExceptionException(ase);
            }
            catch (AmazonClientException ace)
            {
                AmazonExceptionHandlers.HandleAmazonClientExceptionException(ace);
            }
            return StatusCode(500);
        }

        [HttpPut("{tableName}")]
        public async Task<IActionResult> Update([FromBody] UpdateTableRequest request, string tableName)
        {
            request.TableName = tableName;
            Console.WriteLine("Request Info:");
            Console.WriteLine("\tTableName:");
            Console.WriteLine($"\t{request.TableName}");
            try
            {
                var res = await _dynamoClient.UpdateTableAsync(request);
                return new JsonResult(
                    new
                    {
                        message = $"Updated {res.TableDescription.TableName} table.",
                        TableDescription = res.TableDescription
                    })
                { StatusCode = 202 };
            }
            catch (AmazonDynamoDBException addbe)
            {
                return AmazonExceptionHandlers.HandleAmazonDynamoDBException(addbe);
            }
            catch (AmazonServiceException ase)
            {
                AmazonExceptionHandlers.HandleAmazonServiceExceptionException(ase);
            }
            catch (AmazonClientException ace)
            {
                AmazonExceptionHandlers.HandleAmazonClientExceptionException(ace);
            }
            return StatusCode(500);
        }
        [HttpDelete("{tableName}")]
        public async Task<IActionResult> Delete(string tableName)
        {
            System.Console.WriteLine($"Deleting {tableName} table.");
            var req = new DeleteTableRequest();
            req.TableName = tableName;

            try
            {
                var res = await _dynamoClient.DeleteTableAsync(req);
                Console.WriteLine($"Deleted table: {req.TableName}");
                return StatusCode(204);
            }
            catch (AmazonDynamoDBException addbe)
            {
                return AmazonExceptionHandlers.HandleAmazonDynamoDBException(addbe);
            }
            catch (AmazonServiceException ase)
            {
                AmazonExceptionHandlers.HandleAmazonServiceExceptionException(ase);
            }
            catch (AmazonClientException ace)
            {
                AmazonExceptionHandlers.HandleAmazonClientExceptionException(ace);
            }
            return StatusCode(500);
        }
    }
}