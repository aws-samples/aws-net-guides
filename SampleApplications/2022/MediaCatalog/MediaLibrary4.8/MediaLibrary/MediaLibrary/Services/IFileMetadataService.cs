using MediaLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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
