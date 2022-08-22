namespace Common.Model;

public class TextDocument
{
    private List<TextBlock> TextBlocks { get; } = new();

    public void AddText(string text) => TextBlocks.Add(new(text));

    public IEnumerable<string?> GetDocumentParagraphs(bool addParagraphBreaks = false)
    {
        foreach (var block in TextBlocks)
        {
            yield return block.Text;
            if (addParagraphBreaks)
            {
                yield return Environment.NewLine;
            }
        }
    }

    public string GetDocument(bool addParagraphBreaks = false)
    {
        StringBuilder bldr = new();
        foreach (var str in GetDocumentParagraphs(addParagraphBreaks))
        {
            bldr.Append(str);
        }
        return bldr.ToString();
    }
}
