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
        public BookingController(IRoomService roomService, IMapper mapper,IBookingService bookingService, UserManager<AppUser> userManager)
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
                    CheckIn = DateTime.Today.AddDays(1),
                    CheckOut = DateTime.Today.AddDays(2)
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
                var availableRooms = await _roomService.GetAvailableRoomsAsync(dto.CheckIn, dto.CheckOut, dto.Adults, dto.Children);

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
                //if (dto.CheckOut <= dto.CheckIn)
                //{
                //    ModelState.AddModelError("", "Ngày trả phòng phải sau ngày nhận phòng.");
                //}

                //if (!ModelState.IsValid)
                //{
                //    dto.AvailableRooms = await GetAvailableRooms(dto.CheckIn, dto.CheckOut);
                //    return View(dto);
                //}

                //var user = await _userManager.GetUserAsync(User);
                //if (user == null) return Unauthorized();

                //var booking = new Booking
                //{
                //    BookingDate = DateTime.Now,
                //    CheckIn = dto.CheckIn,
                //    CheckOut = dto.CheckOut,
                //    CustomerId = user.Id,
                //    Status = "Pending",
                //    RoomQuantity = dto.SelectedRoomIds.Count,
                //    TotalPrice = 0 
                //};

                //_context.Bookings.Add(booking);
                //await _context.SaveChangesAsync();

                //foreach (var roomId in dto.SelectedRoomIds)
                //{
                //    _context.Booking_Rooms.Add(new Booking_Room
                //    {
                //        BookingID = booking.BookingID,
                //        RoomID = roomId
                //    });
                //}

                //await _context.SaveChangesAsync();
                return RedirectToAction("MyBookings");
            }

            //private async Task<List<Room>> GetAvailableRooms(DateTime checkIn, DateTime checkOut)
            //{
            //    var bookedRoomIds = await _context.Booking_Rooms
            //        .Where(br =>
            //            br.Booking.Status == "Confirmed" &&
            //            (checkIn < br.Booking.CheckOut && checkOut > br.Booking.CheckIn))
            //        .Select(br => br.RoomID)
            //        .Distinct()
            //        .ToListAsync();

            //    return await _context.Rooms
            //        .Include(r => r.RoomType)
            //        .Where(r =>
            //            !bookedRoomIds.Contains(r.RoomID) &&
            //            r.RoomStatus == RoomStatus.Available &&
            //            !r.IsDeleted)
            //        .ToListAsync();
            //}
        }

    }

