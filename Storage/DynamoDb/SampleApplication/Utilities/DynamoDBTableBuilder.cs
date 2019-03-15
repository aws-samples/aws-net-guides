using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBWebApiSample.Utilities
{
    public class DynamoDBTableBuilder
    {
        private IAmazonDynamoDB _dynamoClient;
        public const string TableName = "DocumentProducts";
        public DynamoDBTableBuilder(IAmazonDynamoDB dynamo)
        {
            _dynamoClient = dynamo;
        }

        public async Task<Table> Build()
        {
            var req = new CreateTableRequest();
            req.TableName = TableName;
            req.KeySchema = new List<KeySchemaElement>()
            {
                new KeySchemaElement() { AttributeName = "DocumentProductId",  KeyType = KeyType.HASH },
                new KeySchemaElement() { AttributeName = "PublishOn",  KeyType = KeyType.RANGE }
            };
            req.AttributeDefinitions = new List<AttributeDefinition>()
            {
                new AttributeDefinition() { AttributeName = "DocumentProductId",  AttributeType = "S" },
                new AttributeDefinition() { AttributeName = "PublishOn",  AttributeType = "S" }
            };
            req.ProvisionedThroughput = new ProvisionedThroughput()
            {
                ReadCapacityUnits = 5,
                WriteCapacityUnits = 5
            };
            try
            {
                var res = await _dynamoClient.CreateTableAsync(req);
            }
            catch (AmazonDynamoDBException addbe)
            {
                if (addbe.ErrorCode != "ResourceInUseException") throw;
            }
            return Table.LoadTable(_dynamoClient, TableName);
        }
    }
}