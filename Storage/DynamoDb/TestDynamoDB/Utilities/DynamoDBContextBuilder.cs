using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Configuration;

namespace TestDynamoDB.Utilities
{
    public class DynamoDBContextBuilder
    {
        private IAmazonDynamoDB _dynamoClient;
        public const string TableName = "ObjectPersistenceProducts";
        public DynamoDBContextBuilder(IAmazonDynamoDB dynamo)
        {
            _dynamoClient = dynamo;
        }

        public async Task<DynamoDBContext> Build()
        {
            var req = new CreateTableRequest();
            req.TableName = TableName;
            req.KeySchema = new List<KeySchemaElement>()
            {
                new KeySchemaElement() { AttributeName = "ObjectPersistenceProductId",  KeyType = KeyType.HASH },
                new KeySchemaElement() { AttributeName = "PublishOn",  KeyType = KeyType.RANGE }
            };
            req.AttributeDefinitions = new List<AttributeDefinition>()
            {
                new AttributeDefinition() { AttributeName = "ObjectPersistenceProductId",  AttributeType = "S" },
                new AttributeDefinition() { AttributeName = "PublishOn",  AttributeType = "S" },
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
            return new DynamoDBContext(_dynamoClient);
        }
    }
}