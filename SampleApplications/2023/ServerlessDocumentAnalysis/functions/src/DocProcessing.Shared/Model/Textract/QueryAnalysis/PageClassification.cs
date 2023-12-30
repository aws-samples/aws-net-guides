using System.Text.Json.Serialization;

namespace DocProcessing.Shared.Model.Textract.QueryAnalysis;

public class PageClassification
{
    [JsonPropertyName("PageNumber")]
    public List<Prediction> PageNumber { get; set; } = [];

    [JsonPropertyName("PageType")]
    public List<Prediction> PageType { get; set; } = [];

}
