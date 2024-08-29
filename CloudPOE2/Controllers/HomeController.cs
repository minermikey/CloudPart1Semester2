// Using statements to include necessary namespaces
using CloudPOE2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CloudPOE2.Controllers
{
    public class HomeController : Controller
    {
        // Logger instance to log messages and errors
        private readonly ILogger<HomeController> _logger;

        // Constructor that initializes the logger through dependency injection
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Action method to handle requests to the home page
        public IActionResult Index()
        {
            return View(); // Return the Index view
        }

        // Action method to handle requests to the privacy page
        public IActionResult Privacy()
        {
            return View(); // Return the Privacy view
        }

        // Action method to handle errors
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Return the Error view with an ErrorViewModel containing the request ID
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
