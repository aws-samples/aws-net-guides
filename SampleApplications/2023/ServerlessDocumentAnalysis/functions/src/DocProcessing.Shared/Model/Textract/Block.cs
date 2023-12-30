using System.Text.Json.Serialization;

namespace DocProcessing.Shared.Model.Textract;

public class Block
{
    [JsonPropertyName("BlockType")]
    public string BlockType { get; set; }

    [JsonPropertyName("Geometry")]
    public Geometry Geometry { get; set; }

    [JsonPropertyName("Id")]
    public string Id { get; set; }

    [JsonPropertyName("Relationships")]
    public List<Relationship> Relationships { get; set; }

    [JsonPropertyName("Confidence")]
    public double? Confidence { get; set; }

    [JsonPropertyName("Text")]
    public string Text { get; set; }

    public List<string> GetRelationshipsByType(string relationshipType) => Relationships?.Where(r => r.Type == relationshipType).SelectMany(r => r.Ids).ToList() ?? [];

}