using HomestayBooking.Models;
using HomestayBooking.Models.DAL;
using Microsoft.EntityFrameworkCore;

namespace HomestayBooking.Repositories
{
    public class RoomRepository : BaseRepository<Room>, IRoomRepository
    {
        public RoomRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> DeleteRoom(int roomId)
        {
            return await DeleteAsync(roomId);
        }

        public async Task<List<Room>> GetAllWithRoomType()
        {
            return await _appDbContext.Rooms
                .Where(r => !r.IsDeleted) 
                .Include(r => r.RoomType)
                .ToListAsync();
        }

        public async Task<List<Room>> GetAvailableRooms(DateTime checkIn, DateTime checkOut)
        {
            var bookedRoomIds = await _appDbContext.Booking_Rooms
                .Where(br =>
                    br.Booking.Status == "Confirmed" &&
                    (checkIn < br.Booking.CheckIn && checkOut > br.Booking.CheckIn))
                .Select(br => br.RoomID)
                .Distinct()
                .ToListAsync();

            return await _appDbContext.Rooms
                .Include(r => r.RoomType)
                .Where(r =>
                    !bookedRoomIds.Contains(r.RoomID) &&
                    r.RoomStatus == true &&
                    !r.IsDeleted)
                .ToListAsync();
        }

    }
}
