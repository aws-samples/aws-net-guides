using System.Text.Json.Serialization;

namespace DocProcessing.Shared.Model.Textract;

public class DocumentMetadata
{
    [JsonPropertyName("Pages")]
    public int? Pages { get; set; }
}
