using AutoMapper;
using HomestayBooking.DTOs.BookingDto;
using HomestayBooking.Models;
using HomestayBooking.Models.DAL;
using HomestayBooking.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HomestayBooking.Controllers
{
    public class BookingController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IBookingService _bookingService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        public BookingController(IRoomService roomService, IMapper mapper, IBookingService bookingService, UserManager<AppUser> userManager)
        {
            _roomService = roomService;
            _mapper = mapper;
            _bookingService = bookingService;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateBookingDto
            {
                CheckInDate = DateTime.Today.AddDays(1),
                CheckOutDate = DateTime.Today.AddDays(2)
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckAvailability(CreateBookingDto dto)
        {
            if (!ModelState.IsValid)
                return View("Index", dto); // hoặc return về cùng view hiện tại nếu có lỗi

            // Gọi service để lấy danh sách phòng trống theo điều kiện
            var availableRooms = await _roomService.GetAvailableRoomsAsync(dto.CheckInDate, dto.CheckOutDate, dto.Adults, dto.Children);

            // Lấy danh sách các loại phòng từ các phòng còn trống
            var availableRoomTypes = availableRooms
                .GroupBy(r => r.RoomTypeID)
                .Select(g => g.First().RoomType)
                .ToList();

            // Lưu vào TempData hoặc chuyển qua ViewModel
            TempData["AvailableRoomTypes"] = JsonConvert.SerializeObject(availableRoomTypes);

            return RedirectToAction("Rooms", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBookingDto dto)
        {
            var result = await _bookingService.CreateBooking(dto);
            if (result)
            {
                TempData["SuccessMessage"] = "Đặt phòng thành công!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["ErrorMessage"] = "Đặt phòng không thành công. Vui lòng thử lại sau.";
                return View(dto);
            }
        }

    }
}

