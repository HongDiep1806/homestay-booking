using HomestayBooking.DTOs.RoomDto;
using HomestayBooking.Models;
using HomestayBooking.Repositories;

namespace HomestayBooking.Service
{
    public interface IRoomService
    {
        Task<List<RoomDto>> GetAll();
        Task<bool> Create(Room room);
    }
}
