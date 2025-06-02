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

        public async Task<List<Booking>> GetAllBooking()
        {
            return await _appDbContext.Bookings
                .Where(b => !b.IsDeleted)
                .Include(b => b.RoomType)
                .Include(b => b.Customer)
                .Include(b => b.Staff)
                .ToListAsync();
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
                StaffId = dto.StaffId,
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

        public async Task<bool> CreateBookingByStaffAsync(CreateBookingByStaffDto dto)
        {
            var staffId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(staffId))
                throw new UnauthorizedAccessException("Bạn chưa đăng nhập.");

            var roomType = await _roomTypeRepository.GetById(dto.RoomTypeID);
            if (roomType == null)
                throw new ArgumentException("Loại phòng không tồn tại.");
            var availableRoomTypeIds = await GetAvailableRoomTypeIdsAsync(dto.CheckInDate, dto.CheckOutDate, 1, 0, dto.RoomQuantity);
            if (availableRoomTypeIds == null || !availableRoomTypeIds.Any())
            {
                throw new InvalidOperationException("Hết phòng rồi bé ơi.");
            }

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
                CustomerId = dto.CustomerId,
                StaffId = staffId,
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

            await Create(booking);
            return true;
        }


        public async Task<List<int>> GetAvailableRoomTypeIdsAsync(
    DateTime checkIn, DateTime checkOut, int adults, int childrens, int roomQuantity)
        {
            int totalGuests = adults + childrens;
            Console.WriteLine("checkin date input : " + checkIn);
            Console.WriteLine("checkout date input : " + checkOut);

            // Lấy các RoomID đã được đặt trong khoảng ngày checkIn–checkOut
            var bookedRoomIds = await (from br in _appDbContext.Booking_Rooms
                                       join b in _appDbContext.Bookings on br.BookingID equals b.BookingID
                                       where
                                           (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed) &&
                                           b.CheckOut > checkIn && b.CheckIn < checkOut
                                       select br.RoomID)
                           .Distinct()
                           .ToListAsync();



            Console.WriteLine("=== Booked Room IDs ===");
            foreach (var roomId in bookedRoomIds)
                Console.WriteLine($"RoomID booked: {roomId}");


            // Lấy các RoomType đủ sức chứa khách
            var roomTypes = await _appDbContext.RoomTypes
                .Where(rt => rt.Capacity >= totalGuests)
                .ToListAsync();

            var roomTypeIdsWithEnoughRooms = new List<int>();

            foreach (var rt in roomTypes)
            {
                int availableRoomCount = await _appDbContext.Rooms
    .Where(r => r.RoomTypeID == rt.RoomTypeID &&
                !bookedRoomIds.Contains(r.RoomID) &&
                !r.IsDeleted)
    .CountAsync();


                Console.WriteLine($"RoomTypeID: {rt.RoomTypeID}, Available Rooms: {availableRoomCount}");

                if (availableRoomCount >= roomQuantity)
                {
                    roomTypeIdsWithEnoughRooms.Add(rt.RoomTypeID);
                }
            }

            return roomTypeIdsWithEnoughRooms;
        }

    }
}

