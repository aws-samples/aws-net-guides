using System.Text.Json.Serialization;

namespace DocProcessing.Shared.Model.Textract.Expense;

public class GroupProperty
{
    [JsonPropertyName("Types")]
    public List<string> Types { get; set; }

    [JsonPropertyName("Id")]
    public string Id { get; set; }
}