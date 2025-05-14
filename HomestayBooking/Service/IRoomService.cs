using HomestayBooking.DTOs.RoomDto;
using HomestayBooking.Models;
using HomestayBooking.Repositories;

namespace HomestayBooking.Service
{
    public interface IRoomService
    {
        Task<List<RoomDto>> GetAll();
        Task<bool> Create(Room room);
        Task<bool> Update(int id, Room room);
        Task<bool> Delete(int id);
        Task<Room> GetById(int id);
    }
}
