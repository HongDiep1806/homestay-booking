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

        public async Task<List<Room>> GetAllWithRoomType()
        {
            return await _appDbContext.Rooms
                .Include(r => r.RoomType)
                .ToListAsync();
        }
    }
}
