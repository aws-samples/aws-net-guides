using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AIServicesDemo.Pages
{
    public class SamplesModel : PageModel
    {
        private readonly ILogger<SamplesModel> _logger;

        public SamplesModel(ILogger<SamplesModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}