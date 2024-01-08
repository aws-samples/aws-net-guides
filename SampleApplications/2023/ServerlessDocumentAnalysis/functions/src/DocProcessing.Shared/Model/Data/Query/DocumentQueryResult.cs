using Amazon.DynamoDBv2.DataModel;
using System.Text.Json.Serialization;

namespace DocProcessing.Shared.Model.Data.Query;

public class DocumentQueryResult
{
    [JsonPropertyName("confidence")]
    [DynamoDBProperty("confidence")]
    public double? Confidence { get; set; } = 0d;

    [JsonPropertyName("resultText")]
    [DynamoDBProperty("resultText")]
    public string ResultText { get; set; }

}
