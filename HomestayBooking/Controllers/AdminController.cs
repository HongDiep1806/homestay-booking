using Microsoft.AspNetCore.Mvc;

namespace HomestayBooking.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
