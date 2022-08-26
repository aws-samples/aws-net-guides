using System.Threading.Tasks;
using System.Web;

namespace MediaLibrary.Services
{
    public interface IStorageService
    {
        string SaveFile(HttpPostedFileBase file);

        Task<string[]> GetFileList();

        Task<int> PurgeFiles();

        Task<bool> DeleteFile(string KeyName);
    }
}
