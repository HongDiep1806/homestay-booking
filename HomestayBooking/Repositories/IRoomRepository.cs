using HomestayBooking.Models;

namespace HomestayBooking.Repositories
{
    public interface IRoomRepository : IBaseRepository<Room>
    {
        Task<List<Room>> GetAllWithRoomType();
        Task<bool> DeleteRoom(int roomId);
        Task<List<Room>> GetAvailableRooms(DateTime checkIn, DateTime checkOut);
    }
}
