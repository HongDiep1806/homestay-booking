using AutoMapper;
using HomestayBooking.DTOs.RoomDto;
using HomestayBooking.Models;
using HomestayBooking.Repositories;

namespace HomestayBooking.Service
{

    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IMapper _mapper;

        public RoomService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<bool> Create(Room room)
        {
            try
            {
                await _roomRepository.Create(room);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error in RoomService.Create: " + ex.Message);
                return false;
            }
        }


        public async Task<List<RoomDto>> GetAll()
        {
            return _mapper.Map<List<RoomDto>>(await _roomRepository.GetAll());
        }
    }
}
