using Amazon.DynamoDBv2.DataModel;
using System.Text.Json.Serialization;

namespace DocProcessing.Shared.Model.Data.Query;

[DynamoDBTable("QueryData")]
public class DocumentQuery
{
    [JsonPropertyName("query")]
    [DynamoDBHashKey("query")]
    public string QueryId { get; set; }

    [DynamoDBProperty("queryText")]
    [JsonPropertyName("queryText")]
    public virtual string QueryText { get; set; }

    [DynamoDBProperty("result")]
    [JsonPropertyName("result")]
    public List<DocumentQueryResult> Result { get; set; } = [];

    [DynamoDBProperty("isValid")]
    [JsonPropertyName("isValid")]
    public bool IsValid { get; set; }

    [DynamoDBProperty("processed")]
    [JsonPropertyName("processed")]
    public bool Processed { get; set; }

}
