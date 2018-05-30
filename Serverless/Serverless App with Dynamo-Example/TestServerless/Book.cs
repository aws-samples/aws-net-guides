using Amazon.DynamoDBv2.DataModel;

namespace TestServerless
{
    [DynamoDBTable("readingList")]
    public class Book
    {
        [DynamoDBHashKey]
        public string ItemId { get; set; }
        [DynamoDBRangeKey]
        public string Title { get; set; }
    }
}