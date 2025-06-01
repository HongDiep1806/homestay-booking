using HomestayBooking.DTOs.BookingDto;
using HomestayBooking.Models;
using HomestayBooking.Models.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HomestayBooking.Repositories
{
    public class BookingRepository : BaseRepository<Booking>, IBookingRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRoomTypeRepository _roomTypeRepository;

        public BookingRepository(AppDbContext context, IHttpContextAccessor htppContextAccessor, IRoomTypeRepository roomTypeRepository) : base(context)
        {
            _httpContextAccessor = htppContextAccessor;
            _roomTypeRepository = roomTypeRepository;
        }

        public async Task CreateBooking(Booking booking)
        {
            await Create(booking);
        }

        public async Task<bool> CreateBooking(CreateBookingDto dto)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Bạn chưa đăng nhập.");

            var roomType = await _roomTypeRepository.GetById(dto.RoomTypeID);
            if (roomType == null)
                throw new ArgumentException("Loại phòng không tồn tại.");

            var availableRooms = await _appDbContext.Rooms
                .Where(r =>
                    r.RoomTypeID == dto.RoomTypeID &&
                    !r.IsDeleted &&
                    !_appDbContext.Booking_Rooms.Any(br =>
                        br.RoomID == r.RoomID &&
                        br.Booking.Status != BookingStatus.Cancelled &&
                        !(br.Booking.CheckOut <= dto.CheckInDate || br.Booking.CheckIn >= dto.CheckOutDate)
                    ))
                .ToListAsync();

            // Không cần check đủ hay không nữa vì UI đã lọc trước

            var totalNights = (dto.CheckOutDate - dto.CheckInDate).Days;

            var booking = new Booking
            {
                RoomTypeID = dto.RoomTypeID,
                CheckIn = dto.CheckInDate,
                CheckOut = dto.CheckOutDate,
                RoomQuantity = dto.RoomQuantity,
                BookingDate = DateTime.Now,
                CustomerId = userId,
                Status = BookingStatus.Pending,
                TotalPrice = roomType.Price * dto.RoomQuantity * totalNights
            };

            var roomsToAssign = availableRooms.Take(dto.RoomQuantity).ToList();
            foreach (var room in roomsToAssign)
            {
                booking.Booking_Rooms.Add(new Booking_Room
                {
                    RoomID = room.RoomID
                });
            }

            await CreateBooking(booking);
            return true;
        }

        public async Task<List<int>> GetAvailableRoomTypeIdsAsync(DateTime checkIn, DateTime checkOut, int adults, int childrens, int roomQuantity)
        {
            int totalGuests = adults + childrens;

            // Lấy ID các phòng đã được đặt trong khoảng ngày
            var bookedRoomIds = await _appDbContext.Booking_Rooms
                .Where(br =>
                    br.Booking.Status != BookingStatus.Cancelled &&
                    !(br.Booking.CheckOut <= checkIn || br.Booking.CheckIn >= checkOut))
                .Select(br => br.RoomID)
                .ToListAsync();

            // Lấy tất cả các RoomType đáp ứng số khách
            var roomTypes = await _appDbContext.RoomTypes
                .Where(rt => rt.Capacity >= totalGuests)
                .ToListAsync();

            var roomTypeIdsWithEnoughRooms = new List<int>();

            foreach (var rt in roomTypes)
            {
                // Đếm số phòng thuộc loại này, chưa bị book và chưa bị xóa
                int availableRoomCount = await _appDbContext.Rooms
                    .Where(r => r.RoomTypeID == rt.RoomTypeID &&
                                !bookedRoomIds.Contains(r.RoomID) &&
                                !r.IsDeleted)
                    .CountAsync();

                if (availableRoomCount >= roomQuantity)
                {
                    roomTypeIdsWithEnoughRooms.Add(rt.RoomTypeID);
                }
            }

            return roomTypeIdsWithEnoughRooms;
        }


    }
}

