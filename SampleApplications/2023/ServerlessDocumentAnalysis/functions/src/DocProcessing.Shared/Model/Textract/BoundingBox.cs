using System.Text.Json.Serialization;

namespace DocProcessing.Shared.Model.Textract;

public class BoundingBox
{
    [JsonPropertyName("Height")]
    public double? Height { get; set; }

    [JsonPropertyName("Left")]
    public double? Left { get; set; }

    [JsonPropertyName("Top")]
    public double? Top { get; set; }

    [JsonPropertyName("Width")]
    public double? Width { get; set; }
}