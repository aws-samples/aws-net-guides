using DocProcessing.Shared.Model.Data.Query;

namespace DocProcessing.Shared.Service;

public interface IDataService
{
    Task<IEnumerable<DocumentQuery>> GetAllQueries();
    Task<IEnumerable<DocumentQuery>> GetQueries(IEnumerable<string> queryKeys);



    string GenerateId(string id = null);

    Task<List<T>> GetBySingleIndex<T>(string id, string indexName);

    Task<T> GetData<T>(string id);

    Task<T> SaveData<T>(T data);
}
