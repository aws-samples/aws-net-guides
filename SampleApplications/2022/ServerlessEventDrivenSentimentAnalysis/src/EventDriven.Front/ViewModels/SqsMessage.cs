using System.Text.Json.Serialization;

namespace EventDriven.Front;

public class SqsMessage
{
    [JsonPropertyName("Token")]
    public string TaskToken { get; set; }

    public string ReceiptHandle { get; set; }

    public Payload Payload { get; set; }
    
}

public class Payload
{
    [JsonPropertyName("emailAddress")]
    public string EmailAddress { get; set; }
    
    [JsonPropertyName("reviewContent")]
    public string ReviewContent { get; set; }
    
    [JsonPropertyName("originalReviewContents")]
    public string OriginalReviewContents { get; set; }
    
    [JsonPropertyName("reviewId")]
    public string ReviewId { get; set; }
}