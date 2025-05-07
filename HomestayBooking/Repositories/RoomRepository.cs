using HomestayBooking.Models;
using HomestayBooking.Models.DAL;

namespace HomestayBooking.Repositories
{
    public class RoomRepository : BaseRepository<Room>, IRoomRepository
    {
        public RoomRepository(AppDbContext context) : base(context)
        {
        }

    }
}
