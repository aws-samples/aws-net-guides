using Amazon.Textract.Model;

namespace DocProcessing.Shared.AwsSdkUtilities;

[Obsolete("I think this is obsolete")]
public class TextractResult
{
    private List<Block> Blocks { get; } = [];

    public TextractResult(IEnumerable<Block> blocks)
    {
        Blocks.AddRange(blocks);
    }
}
