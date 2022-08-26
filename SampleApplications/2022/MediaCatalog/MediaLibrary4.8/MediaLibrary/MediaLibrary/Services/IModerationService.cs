using MediaLibrary.Models;
using System.Threading.Tasks;

namespace MediaLibrary.Services
{
    public interface IModerationService
    {
        Task<ModerationResultsViewModel> IsContentAllowed(string objectLocation);
    }
}
