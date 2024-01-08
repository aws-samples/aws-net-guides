namespace DocProcessing.Shared.Model.Textract.Expense;

public class ExpenseDataModel(IEnumerable<ExpenseDocument> docs)
{
    Dictionary<int, ExpenseDocument> ExpenseDocuments { get; } = docs.ToDictionary(a => a.ExpenseIndex.GetValueOrDefault()) ?? [];
    private readonly Dictionary<int, HashSet<string>> _groupSummaryFields = [];

    public IEnumerable<int> GetExpenseReportIndexes() => ExpenseDocuments.Keys;

    public IEnumerable<string> GetGroupIds(int expenseDocId)
    {
        if (!ExpenseDocuments.TryGetValue(expenseDocId, out ExpenseDocument doc))
        {
            return Enumerable.Empty<string>();
        }

        if (!_groupSummaryFields.TryGetValue(expenseDocId, out HashSet<string> value))
        {
            value = doc.SummaryFields
            .Where(g => g.GroupProperties is not null)
            .SelectMany(g => g.GroupProperties)
            .Select(a => a.Id)
            .ToHashSet();
            _groupSummaryFields[expenseDocId] = value;
        };

        return value;
    }

    // Get a tuple of Groups and Types for a Node
    public IEnumerable<(string, string)> GetGroupTypes(int expenseDocId)
    {
        foreach (string g in GetGroupIds(expenseDocId))
        {
            foreach (string t in GetTypesForGroup(expenseDocId, g))
            {
                yield return (g, t);
            }
        };
    }

    public IEnumerable<string> GetTypesForGroup(int expenseDocId, string groupId)
    {
        if (!ExpenseDocuments.TryGetValue(expenseDocId, out ExpenseDocument value) || !GetGroupIds(expenseDocId).Contains(groupId))
        {
            return Enumerable.Empty<string>();
        }

        return value.SummaryFields
            .Where(g => g.GroupProperties is not null && g.GroupProperties.Any(a => a.Id == groupId))
            .SelectMany(g => g.GroupProperties)
            .SelectMany(g => g.Types)
            .ToHashSet();
    }




    public IEnumerable<SummaryField> GetGroupSummaryFields(int expenseDocId, string groupId, string type)
    {
        if (!ExpenseDocuments.TryGetValue(expenseDocId, out ExpenseDocument value) || !GetGroupIds(expenseDocId).Contains(groupId))
        {
            return Enumerable.Empty<SummaryField>();
        }

        return value.SummaryFields
            .Where(g => g.GroupProperties is not null && g.GroupProperties.Any(a => a.Id == groupId && a.Types.Any(b => b == type)));
    }

    public IEnumerable<SummaryField> GetScalarSummaryFields(int expenseDocId)
    {
        if (!ExpenseDocuments.TryGetValue(expenseDocId, out ExpenseDocument value))
        {
            return Enumerable.Empty<SummaryField>();
        }

        return value.SummaryFields.Where(g => g.GroupProperties is null);
    }

}