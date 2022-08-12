namespace MediaLibrary.Services
{
    public interface IStorageService
    {
        string SaveFile(IFormFile file);

        Task<string[]> GetFileList();

        Task<int> PurgeFiles();

        Task<bool> DeleteFile(string KeyName);
    }
}
