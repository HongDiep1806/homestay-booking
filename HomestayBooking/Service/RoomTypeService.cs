using HomestayBooking.Models;
using HomestayBooking.Repositories;

namespace HomestayBooking.Service
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IRoomTypeRepository _roomTypeRepository;

        public RoomTypeService(IRoomTypeRepository roomTypeRepository)
        {
            _roomTypeRepository = roomTypeRepository;
        }
        public async Task<List<RoomType>> GetAll()
        {
            return await _roomTypeRepository.GetAll();
        }

        public async Task<RoomType> GetById(int id)
        {
            return await _roomTypeRepository.GetById(id);
        }

        public async Task<List<RoomType>> GetByIds(List<int> ids)
        {
            var allRoomTypes = await _roomTypeRepository.GetAll();
            return allRoomTypes
                .Where(rt => ids.Contains(rt.RoomTypeID) && !rt.IsDeleted)
                .ToList();
        }

    }
}
