using System.Text.Json.Serialization;

namespace DocProcessing.Shared.Model.Textract.QueryAnalysis;

public class Query
{
    [JsonPropertyName("Text")]
    public string Text { get; set; }

    [JsonPropertyName("Alias")]
    public string Alias { get; set; } = string.Empty;

    [JsonPropertyName("Pages")]
    public List<string> Pages { get; set; } = [];
}
