using HomestayBooking.Models;

namespace HomestayBooking.Service
{
    public interface IRoomTypeService
    {
        Task<List<RoomType>> GetAll();
    }
}
