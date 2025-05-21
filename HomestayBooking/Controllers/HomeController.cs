using System.Diagnostics;
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
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, IRoomService roomService, IBookingService bookingService, UserManager<AppUser> userManager, IMapper mapper)
        {
            _logger = logger;
            _roomService = roomService;
            _bookingService = bookingService;
            _userManager = userManager;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var model = new CreateBookingDto
            {
                CheckIn = DateTime.Today.AddDays(1),
                CheckOut = DateTime.Today.AddDays(2),
                AvailableRooms = new List<Room>()
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
        public IActionResult Rooms()
        {
            TempData.Keep("AvailableRoomTypes"); // giữ lại để F5 vẫn còn dùng

            if (TempData["AvailableRoomTypes"] != null)
            {
                var json = TempData["AvailableRoomTypes"] as string;
                var availableRoomTypes = JsonConvert.DeserializeObject<List<RoomType>>(json);
                Console.WriteLine(">>>>>> Có room type: " + availableRoomTypes.Count);
                return View(availableRoomTypes);
            }

            Console.WriteLine(">>>>>> Không có TempData");
            return View(new List<RoomType>());
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
        public IActionResult RoomDetails()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckAvailability(CreateBookingDto dto)
        {
            if (!ModelState.IsValid)
                return View("Index", dto); // hoặc return về cùng view hiện tại nếu có lỗi

            // Gọi service để lấy danh sách phòng trống theo điều kiện
            var availableRooms = await _roomService.GetAvailableRoomsAsync(dto.CheckIn, dto.CheckOut, dto.Adults, dto.Children);

            // Lấy danh sách các loại phòng từ các phòng còn trống
            var availableRoomTypes = availableRooms
                .GroupBy(r => r.RoomTypeID)
                .Select(g => g.First().RoomType)
                .ToList();
            if(availableRoomTypes.Count == 0)
            {
               Console.WriteLine("Không có phòng nào trống trong khoảng thời gian đã chọn.");   
                return View("Index", dto);
            }

            // Lưu vào TempData hoặc chuyển qua ViewModel
            TempData["AvailableRoomTypes"] = JsonConvert.SerializeObject(availableRoomTypes);

            return RedirectToAction("Rooms", "Home");
        }


    }
}
