using MediaLibrary.Models;

namespace MediaLibrary.Services
{
    public interface IImageLookupService
    {
        void SaveLookupData(ImageLookupDataModel data);

        Task<ImageLookupDataModel> GetLookupData(string itemId);

        Task<int> RemoveImageFromLookups(string imageName);

        Task<IEnumerable<ImageLookupDataModel>> GetLabelData();

    }
}
