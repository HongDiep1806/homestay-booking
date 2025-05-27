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

            var availableRooms = await GetAvailableRoomsAsync(
                dto.CheckInDate,
                dto.CheckOutDate,
                dto.Children,
                dto.Adults
            );

            if (availableRooms.Count < dto.RoomQuantity)
                throw new InvalidOperationException("Không đủ phòng trống.");

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

            foreach (var room in availableRooms)
            {
                booking.Booking_Rooms.Add(new Booking_Room
                {
                    RoomID = room.RoomID
                });
            }

            await CreateBooking(booking);

            return true;
        }

        public async Task<List<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int adults, int childrens)
        {
            int totalGuests = childrens + adults;
            var bookedRoomIds = await _appDbContext.Booking_Rooms
                .Where(br =>
                    br.Booking.Status != BookingStatus.Cancelled &&
                    !(br.Booking.CheckOut <= checkIn || br.Booking.CheckIn >= checkOut))
                .Select(br => br.RoomID)
                .Distinct()
                .ToListAsync();
            var validRoomTypeIds = await _appDbContext.RoomTypes.Where(r => r.Capacity >= totalGuests)
                .Select(r => r.RoomTypeID).ToListAsync();
            var availableRooms = await _appDbContext.Rooms
                    .Include(r => r.RoomType)
                    .Where(r => !bookedRoomIds.Contains(r.RoomID) &&
                    validRoomTypeIds.Contains(r.RoomTypeID) &&
                    !r.IsDeleted)
                    .ToListAsync();

            return availableRooms;
        }

    }
}

