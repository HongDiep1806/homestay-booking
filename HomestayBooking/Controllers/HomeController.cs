using System.Diagnostics;
using HomestayBooking.Models;
using Microsoft.AspNetCore.Mvc;

namespace HomestayBooking.Controllers
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
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult NotFound404()
        {
            return View();
        }
        public IActionResult Rooms()
        {
            return View();
        }
        public IActionResult Restaurant()
        {
            return View();
        }
        public IActionResult Spa()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult News()
        {
            return View();
        }
        public IActionResult Service()
        {
            return View();
        }

    }
}
