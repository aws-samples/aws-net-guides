using MediaLibrary.Models;

namespace MediaLibrary.Services
{
    public interface IImageMetadataService
    {
        void SaveImageData(ImageMetadataDataModel data);

        Task<ImageMetadataDataModel> GetImageData(string itemId);

        Task<bool> DeleteImageData(ImageMetadataDataModel imageData);
    }
}
