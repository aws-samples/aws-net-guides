using Amazon.Textract.Model;
using Common.Model;

namespace Common.Services;

public interface ITextToSpeechUtilities
{
    TextDocument GetTextDocument(List<Block> blocks);

}
