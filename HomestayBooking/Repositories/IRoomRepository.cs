using HomestayBooking.Models;

namespace HomestayBooking.Repositories
{
    public interface IRoomRepository : IBaseRepository<Room>
    {
        Task<List<Room>> GetAllWithRoomType();
    }
}
