using System.Text.Json.Serialization;

namespace DocProcessing.Shared.Model.Textract.QueryAnalysis;

public class TextractAnalysisResult
{
    [JsonPropertyName("AnalyzeDocumentModelVersion")]
    public string AnalyzeDocumentModelVersion { get; set; }

    [JsonPropertyName("Blocks")]
    public List<Block> Blocks { get; set; } = [];

    [JsonPropertyName("DocumentMetadata")]
    public DocumentMetadata DocumentMetadata { get; set; }

    [JsonPropertyName("JobStatus")]
    public string JobStatus { get; set; }

    [JsonPropertyName("StatusMessage")]
    public string StatusMessage { get; set; }

    public int GetBlockCount() => Blocks.Count;
}
