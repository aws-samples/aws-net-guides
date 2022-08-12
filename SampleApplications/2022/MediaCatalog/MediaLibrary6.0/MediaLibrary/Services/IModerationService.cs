using MediaLibrary.Models;

namespace MediaLibrary.Services
{
    public interface IModerationService
    {
        Task<ModerationResultsViewModel> IsContentAllowed(string objectLocation);
    }
}
