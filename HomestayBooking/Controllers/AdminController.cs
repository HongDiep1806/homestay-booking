using HomestayBooking.Models;
using HomestayBooking.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HomestayBooking.Controllers
{
    public class AdminController : Controller
    {
        private readonly IRoomTypeService _roomTypeService;
        private readonly IRoomService _roomService;
        public AdminController(IRoomTypeService roomTypeService, IRoomService roomService)
        {
            _roomTypeService = roomTypeService;
            _roomService = roomService;
        }
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

        public IActionResult AllCustomer()
        {
            return View();
        }

        public IActionResult AddCustomer()
        {
            return View();
        }

        public IActionResult EditCustomer()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> AllRoom()
        {
            var rooms = await _roomService.GetAll();
            return View(rooms);
        }

        [HttpGet]
        public async Task<IActionResult> AddRoom()
        {
            var roomTypes = await _roomTypeService.GetAll();
            ViewBag.RoomTypes = roomTypes;    
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRoom(Room room, IFormFile RoomImageFile)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(room.RoomCode))
                errors.Add("Room code is required.");

            if (room.RoomTypeID <= 0)
                errors.Add("Room type must be selected.");

            if (RoomImageFile == null || RoomImageFile.Length == 0)
                errors.Add("An image file must be uploaded.");

            if (errors.Any())
            {
                ViewBag.Errors = errors;
                ViewBag.RoomTypes = await _roomTypeService.GetAll();
                return View(room);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(RoomImageFile.FileName);
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await RoomImageFile.CopyToAsync(stream);
            }

            room.RoomImg = "/img/" + fileName;
            room.RoomStatus = true;

            await _roomService.Create(room);

            return RedirectToAction("AllRoom");
        }


        [HttpGet]
        public async Task<IActionResult> EditRoom(int id)
        {
            var room = await _roomService.GetById(id);
            if (room == null)
                return NotFound();
            var roomTypes = await _roomTypeService.GetAll();
            ViewBag.RoomTypes = new SelectList(roomTypes, "RoomTypeID", "Name", room.RoomTypeID);
            return View(room);
        }

        [HttpPost]
        public async Task<IActionResult> EditRoom(Room room, IFormFile? RoomImageFile)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(room.RoomCode))
                errors.Add("Room code is required.");
            if (room.RoomTypeID <= 0)
                errors.Add("Room type must be selected.");
            if (errors.Any())
            {
                ViewBag.Errors = errors;
                ViewBag.RoomTypes = await _roomTypeService.GetAll();
                return View(room);
            }
            if (RoomImageFile != null && RoomImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(RoomImageFile.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);
                var filePath = Path.Combine(folderPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await RoomImageFile.CopyToAsync(stream);
                }
                room.RoomImg = "/img/" + fileName;
            }
            await _roomService.Update(room.RoomID, room);
            return RedirectToAction("AllRoom");
        }

        public IActionResult AllStaff()
        {
            return View();
        }

        public IActionResult AddStaff()
        {
            return View();
        }

        public IActionResult EditStaff()
        {
            return View();
        }

        public IActionResult Invoice()
        {
            return View();
        }

        public IActionResult CreateInvoice()
        {
            return View();
        }

        public IActionResult InvoiceReport()
        {
            return View();
        }

        public IActionResult AllBlog()
        {
            return View();
        }

        public IActionResult AddBlog()
        {
            return View();
        }

        public IActionResult EditBlog()
        {
            return View();
        }

        public IActionResult BlogDetail()
        {
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult EditProfile()
        {
            return View();
        }

        public IActionResult Error404()
        {
            return View();
        }

        public IActionResult Error500()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var result = await _roomService.Delete(id);
            if (!result)
            {
                TempData["Error"] = "Failed to delete room.";
            }
            return RedirectToAction("AllRoom");
        }


    }
}
