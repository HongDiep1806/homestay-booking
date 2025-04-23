using Microsoft.AspNetCore.Mvc;

namespace HomestayBooking.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AllBooking()
        {
            return View();
        }

        public IActionResult AddBooking()
        {
            return View();
        }

        public IActionResult EditBooking()
        {
            return View();
        }
    }
}
