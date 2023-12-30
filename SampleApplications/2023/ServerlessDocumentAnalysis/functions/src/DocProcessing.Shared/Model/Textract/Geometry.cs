using System.Text.Json.Serialization;

namespace DocProcessing.Shared.Model.Textract;

public class Geometry
{
    [JsonPropertyName("BoundingBox")]
    public BoundingBox BoundingBox { get; set; }

    [JsonPropertyName("Polygon")]
    public List<Polygon> Polygon { get; set; }
}