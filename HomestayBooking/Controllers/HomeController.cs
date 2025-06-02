using System.Diagnostics;
using System.Globalization;
using AutoMapper;
using HomestayBooking.DTOs.BookingDto;
using HomestayBooking.Models;
using HomestayBooking.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HomestayBooking.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRoomService _roomService;
        private readonly IBookingService _bookingService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IRoomTypeService _roomTypeService;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, IRoomTypeService roomTypeService, IRoomService roomService, IBookingService bookingService, UserManager<AppUser> userManager, IMapper mapper)
        {
            _logger = logger;
            _roomService = roomService;
            _bookingService = bookingService;
            _userManager = userManager;
            _mapper = mapper;
            _roomTypeService = roomTypeService;
        }

        public IActionResult Index()
        {
            var model = new CheckingAvailableRoomDto
            {
                CheckIn = DateTime.Today.AddDays(1).Date.AddHours(12), 
                CheckOut = DateTime.Today.AddDays(2).Date.AddHours(12),
                Adults = 1,
                Children = 0,
            };

            return View(model);
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
        public async Task<IActionResult> Rooms(string idsJson, string checkIn, string checkOut, int roomQuantity)
        {
            if (string.IsNullOrEmpty(idsJson) || string.IsNullOrEmpty(checkIn) || string.IsNullOrEmpty(checkOut))
            {
                return RedirectToAction("Index");
            }

            var ids = JsonConvert.DeserializeObject<List<int>>(idsJson);
            var parsedCheckIn = DateTime.ParseExact(checkIn, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var parsedCheckOut = DateTime.ParseExact(checkOut, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            ViewBag.CheckIn = parsedCheckIn;
            ViewBag.CheckOut = parsedCheckOut;
            ViewBag.RoomQuantity = roomQuantity;

            var roomTypes = await _roomTypeService.GetByIds(ids);
            return View(roomTypes);
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
        //public IActionResult RoomDetails()
        //{
        //    return View();
        //}
        public async Task<IActionResult> RoomDetails(int id)
        {
            var roomType = await _roomTypeService.GetById(id);

            return View(roomType);
        }

        [HttpPost]
        public async Task<IActionResult> CheckAvailability([FromBody] CheckingAvailableRoomDto dto)
        {
            var roomTypeIds = await _bookingService.GetAvailableRoomTypeIdsAsync(
                dto.CheckIn,
                dto.CheckOut,
                dto.Adults,
                dto.Children,
                dto.RoomQuantity
            );

            if (!roomTypeIds.Any())
            {
                return Json(new
                {
                    success = false,
                    message = "Không còn phòng trống phù hợp với yêu cầu của bạn !"
                });
            }

            var redirectUrl = Url.Action("Rooms", new
            {
                idsJson = JsonConvert.SerializeObject(roomTypeIds),
                checkIn = dto.CheckIn.ToString("yyyy-MM-dd"),
                checkOut = dto.CheckOut.ToString("yyyy-MM-dd"),
                roomQuantity = dto.RoomQuantity
            });

            return Json(new
            {
                success = true,
                redirectUrl
            });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBooking(CreateBookingDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", new { error = "Bạn cần đăng nhập để đặt phòng." });
            }

            dto.UserId = user.Id; // 👈 Gán ID người dùng đang đăng nhập

            var result = await _bookingService.CreateBooking(dto);
            if (result)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index", new { error = "Đặt phòng không thành công. Vui lòng thử lại." });
        }


    }
}
