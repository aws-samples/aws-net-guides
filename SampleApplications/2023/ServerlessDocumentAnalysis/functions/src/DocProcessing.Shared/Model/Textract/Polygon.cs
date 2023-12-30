using System.Text.Json.Serialization;

namespace DocProcessing.Shared.Model.Textract;

public class Polygon
{
    [JsonPropertyName("X")]
    public double? X { get; set; }

    [JsonPropertyName("Y")]
    public double? Y { get; set; }
}