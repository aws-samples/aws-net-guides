using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MediaLibrary.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Image index and processing application. (Media Catalog) services provided by Amazon Rekognition!";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Our contact details:";
            return View();
        }
    }
}