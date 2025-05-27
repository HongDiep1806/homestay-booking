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
            var model = new CreateBookingDto
            {
                CheckInDate = DateTime.Today.AddDays(1).Date.AddHours(12), 
                CheckOutDate = DateTime.Today.AddDays(2).Date.AddHours(12),
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
        public async Task<IActionResult> Rooms()
        {
            if (TempData["AvailableRoomTypeIds"] != null)
            {
                var ids = JsonConvert.DeserializeObject<List<int>>(TempData["AvailableRoomTypeIds"].ToString());
                var roomTypes = await _roomTypeService.GetByIds(ids);
                return View(roomTypes); // View nhận List<RoomType>
            }

            return RedirectToAction("Index");
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckAvailability(CheckingAvailableRoomDto dto)
        {
            var availableRooms = await _roomService.GetAvailableRoomsAsync(dto.CheckIn, dto.CheckOut, dto.Adults, dto.Children);

            var roomTypeIds = availableRooms
                .Select(r => r.RoomTypeID)
                .Distinct()
                .ToList();

            if (!roomTypeIds.Any())
            {
                TempData["Message"] = "Không có phòng nào trống.";
                return RedirectToAction("Index");
            }

            TempData["AvailableRoomTypeIds"] = JsonConvert.SerializeObject(roomTypeIds);
            return RedirectToAction("Rooms");
        }
        [HttpPost]
        public async Task<IActionResult> CreateBooking(CreateBookingDto dto)
        {
           var result = await _bookingService.CreateBooking(dto);
            if (result)
            {
                return RedirectToAction("Index");

            }
            return RedirectToAction("Index", new { error = "Đặt phòng không thành công. Vui lòng thử lại." });  
        }



    }
}
