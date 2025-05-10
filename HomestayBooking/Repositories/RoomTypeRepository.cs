using HomestayBooking.Models;
using HomestayBooking.Models.DAL;

namespace HomestayBooking.Repositories
{
    public class RoomTypeRepository : BaseRepository<RoomType>, IRoomTypeRepository
    {
        public RoomTypeRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
