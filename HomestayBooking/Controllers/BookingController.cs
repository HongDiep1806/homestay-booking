using AutoMapper;
using HomestayBooking.DTOs.BookingDto;
using HomestayBooking.Models;
using HomestayBooking.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomestayBooking.Controllers
{
    public class BookingController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;
        public BookingController(IRoomService roomService, IMapper mapper)
        {
            _roomService = roomService;
            _mapper = mapper;
        }
        [HttpGet]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create()
        {
            var rooms = await _roomService.GetAll();

            var model = new CreateBookingDto
            {
                CheckIn = DateTime.Today,
                CheckOut = DateTime.Today.AddDays(1),
                AvailableRooms = _mapper.Map<List<Room>>(rooms) 
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create(CreateBookingDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            if (!ModelState.IsValid)
            {
                dto.AvailableRooms = await _context.Rooms
                    .Include(r => r.RoomType)
                    .Where(r => r.RoomStatus == true && !r.IsDeleted)
                    .ToListAsync();

                return View(dto);
            }

            var booking = new Booking
            {
                BookingDate = DateTime.Now,
                CustomerId = user.Id,
                RoomQuantity = dto.SelectedRoomIds.Count,
                Status = "Pending",
                TotalPrice = 0 // Bạn có thể tính toán dựa vào loại phòng
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            foreach (var roomId in dto.SelectedRoomIds)
            {
                var br = new Booking_Room
                {
                    RoomID = roomId,
                    BookingID = booking.BookingID
                };
                _context.Booking_Rooms.Add(br);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("MyBookings"); // tạo trang này để xem đơn
        }

    }
}
