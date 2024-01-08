using System.Text.Json.Serialization;

namespace DocProcessing.Shared.Model.Textract.QueryAnalysis;

public class Block : DocProcessing.Shared.Model.Textract.Block
{

    [JsonPropertyName("ColumnIndex")]
    public int? ColumnIndex { get; set; }

    [JsonPropertyName("ColumnSpan")]
    public int? ColumnSpan { get; set; }

    [JsonPropertyName("EntityTypes")]
    public List<string> EntityTypes { get; set; } = [];

    [JsonPropertyName("Hint")]
    public string Hint { get; set; }

    [JsonPropertyName("Page")]
    public int? Page { get; set; }

    [JsonPropertyName("PageClassification")]
    public PageClassification PageClassification { get; set; }

    [JsonPropertyName("Query")]
    public Query Query { get; set; } = new();

    [JsonPropertyName("RowIndex")]
    public int? RowIndex { get; set; }

    [JsonPropertyName("RowSpan")]
    public int? RowSpan { get; set; }

    [JsonPropertyName("SelectionStatus")]
    public string SelectionStatus { get; set; }

    [JsonPropertyName("TextType")]
    public string TextType { get; set; }




}
