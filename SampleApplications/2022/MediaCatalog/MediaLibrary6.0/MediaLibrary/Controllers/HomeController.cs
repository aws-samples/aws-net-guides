using Microsoft.AspNetCore.Mvc;
using MediaLibrary.Models;
using System.Diagnostics;

namespace MediaLibrary.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.Log(LogLevel.Information, "HomeController::Index", null);
            return View();
        }

        public IActionResult Privacy()
        {
            _logger.Log(LogLevel.Information, "HomeController::Privacy", null);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.Log(LogLevel.Information, "HomeController::Error", null);
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}