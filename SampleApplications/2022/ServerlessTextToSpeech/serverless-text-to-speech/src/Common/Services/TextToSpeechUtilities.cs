using Amazon.Textract.Model;
using Common.Model;

namespace Common.Services;

public class TextToSpeechUtilities : ITextToSpeechUtilities
{
    public TextDocument GetTextDocument(List<Block> blocks)
    {
        var doc = new TextDocument();


        foreach (var block in blocks)
        {
            if (block.BlockType == Amazon.Textract.BlockType.LINE)
            {
                doc.AddText(block.Text);
            }
        }
        return doc;
    }
}
