using MediaLibrary.Models;

namespace MediaLibrary.Services
{
    public interface IFileMetadataService
    {
        void SaveMetadata(FileMetadataDataModel data);

        Task<IEnumerable<FileMetadataDataModel>> GetFileList();

        Task<FileMetadataDataModel> GetFileMetadata(string itemId);

        Task<bool> DeleteFileMetadata(FileMetadataDataModel imageData);
    }
}
