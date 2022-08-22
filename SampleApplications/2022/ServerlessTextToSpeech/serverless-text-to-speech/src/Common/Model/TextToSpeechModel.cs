using Amazon.DynamoDBv2.DataModel;


namespace Common.Model;

[DynamoDBTable("TextToSpeechData")]
public class TextToSpeechModel
{


    [DynamoDBHashKey("id")]
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("bucket")]
    public string? BucketName { get; set; }

    [JsonPropertyName("key")]
    public string? ObjectKey { get; set; }

    [JsonPropertyName("time")]
    public DateTime? RequestTime { get; set; }

    [JsonPropertyName("region")]
    public string? Region { get; set; }

    [JsonPropertyName("jobid")]
    public string? TextractJobId { get; set; }

    [JsonPropertyName("tasktokenid")]
    public string? TaskToken { get; set; }

    [JsonPropertyName("pollyjobid")]
    public string? PollyJobId { get; set; }

    [JsonPropertyName("pollytasktokenid")]
    public string? PollyTaskToken { get; set; }

    [JsonPropertyName("pollyoutputuri")]
    public string? PollyOutputUri { get; set; }

    [JsonPropertyName("soundbucket")]
    public string? SoundBucket { get; set; }

    [JsonPropertyName("soundkey")]
    public string? SoundKey { get; set; }
}