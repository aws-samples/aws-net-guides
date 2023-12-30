namespace DocProcessing.Shared.Model.Textract.QueryAnalysis;

public class TextractDataModel
{
    private const string QUERY_TYPE = "QUERY";

    Dictionary<string, Block> BlockMap { get; }

    private Dictionary<string, List<Block>> Queries = [];

    public TextractDataModel(IEnumerable<Block> blocks)
    {
        BlockMap = blocks.ToDictionary(a => a.Id) ?? [];
        if (BlockMap.Count > 0)
        {
            Initialize();
        }
    }

    //Initialization
    private void Initialize()
    {

        Queries = BlockMap
            .Values
            .Where(b => b.BlockType == QUERY_TYPE)
            .GroupBy(a => a.Query.Alias)
            .ToDictionary(a => a.Key, b => b.ToList());
    }

    public Block GetBlock(string id)
    {
        if (BlockMap.TryGetValue(id, out var block))
        {
            return block;
        }
        return null;
    }

    public IEnumerable<Block> GetQueryResults(string queryAlias)
    {
        if (!string.IsNullOrEmpty(queryAlias) && Queries.TryGetValue(queryAlias, out var blocks))
        {
            foreach (var blockId in blocks.SelectMany(a => a.GetRelationshipsByType("ANSWER")))
            {
                if (BlockMap.TryGetValue(blockId, out var answerBlock))
                {
                    yield return answerBlock;
                }
            }

        }
    }

    public int BlockCount => BlockMap?.Count ?? 0;
}
