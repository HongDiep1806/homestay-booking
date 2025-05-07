using HomestayBooking.DTOs.RoomDto;
using HomestayBooking.Repositories;

namespace HomestayBooking.Service
{
    public interface IRoomService
    {
        Task<List<RoomDto>> GetAll();   
    }
}
