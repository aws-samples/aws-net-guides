using MediaLibrary.Models;
using System.Threading.Tasks;

namespace MediaLibrary.Services
{
    public interface IImageMetadataService
    {
        void SaveImageData(ImageMetadataDataModel data);

        Task<ImageMetadataDataModel> GetImageData(string itemId);

        Task<bool> DeleteImageData(ImageMetadataDataModel imageData);
    }
}
