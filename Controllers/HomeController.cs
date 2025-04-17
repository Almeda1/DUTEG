using System.Diagnostics;
using DUTEG.Models;
using DUTEG.Services;
using Microsoft.AspNetCore.Mvc;
using DUTEG.Models;

namespace DUTEG.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
      

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Home()
        {
            ViewData["ActivePage"] = "Home";
            return View();
        }

        public IActionResult Products()
        {
            ViewData["ActivePage"] = "Products"; 
            return View();
        }

        // Refund Policy Page
        public IActionResult RefundPolicy()
        {
            ViewData["ActivePage"] = "RefundPolicy";
            return View();
        }

        // Contact Page
        public IActionResult Contact()
        {
            ViewData["ActivePage"] = "Contact";
            return View();
        }

        // About Us Page
        public IActionResult AboutUs()
        {
            ViewData["ActivePage"] = "AboutUs";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

       
    }
}


