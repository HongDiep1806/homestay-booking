using AutoMapper;
using HomestayBooking.DTOs.RoomDto;
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
        public async Task<List<RoomDto>> GetAll()
        {
            return _mapper.Map<List<RoomDto>>(await _roomRepository.GetAll());
        }
    }
}
