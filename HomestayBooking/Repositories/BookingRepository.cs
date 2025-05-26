using HomestayBooking.Models;
using HomestayBooking.Models.DAL;
using Microsoft.EntityFrameworkCore;

namespace HomestayBooking.Repositories
{
    public class BookingRepository : BaseRepository<Booking>, IBookingRepository
    {

        public BookingRepository(AppDbContext context) : base(context)
        {
        }

        public async Task CreateBooking(Booking booking)
        {
            await Create(booking);
        }

        public async Task<List<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int adults, int children)
        {
            // Tổng số người
            int totalGuests = adults + children;

            // Bước 1: Lấy danh sách các RoomId đã bị đặt trong khoảng thời gian đó
            var bookedRoomIds = await _appDbContext.Booking_Rooms
                .Where(br =>
                    br.Booking.Status != "Cancelled" && // nếu có logic hủy
                    !(br.Booking.CheckOut <= checkIn || br.Booking.CheckIn >= checkOut))
                .Select(br => br.RoomID)
                .Distinct()
                .ToListAsync();

            // Bước 2: Lọc các phòng không bị đặt và đủ sức chứa
            var availableRooms = await _appDbContext.Rooms
                .Include(r => r.RoomType)
                .Where(r =>!bookedRoomIds.Contains(r.RoomID))
                .ToListAsync();

            return availableRooms;
        }
    }

}
