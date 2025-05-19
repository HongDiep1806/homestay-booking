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

    }
}
