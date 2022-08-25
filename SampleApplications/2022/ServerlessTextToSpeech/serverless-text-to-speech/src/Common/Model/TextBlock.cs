namespace Common.Model;

public class TextBlock
{

    public TextBlock()
    {

    }

    public TextBlock(string text)
        : this()
    {
        Text = text;
    }
    public string? Text { get; set; }
}
