using Amazon.DynamoDBv2.DataModel;

namespace TestDynamoDB.ObjectPersistenceModels
{
    [DynamoDBTable("ObjectPersistenceProducts")]
    public class Product
    {
        [DynamoDBHashKey]
        public string ObjectPersistenceProductId { get; set; }
        [DynamoDBRangeKey]
        public string PublishOn { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
    }
}