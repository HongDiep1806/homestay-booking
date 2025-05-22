using AutoMapper;
using HomestayBooking.DTOs.RoomDto;
using HomestayBooking.Models;
using HomestayBooking.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HomestayBooking.Service
{

    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IRoomTypeRepository _roomTypeRepository;   
        private readonly IMapper _mapper;

        public RoomService(IRoomRepository roomRepository, IMapper mapper, IRoomTypeRepository roomTypeRepository)
        {
            _roomRepository = roomRepository;
            _mapper = mapper;
            _roomTypeRepository = roomTypeRepository;
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
                Console.WriteLine("Error in RoomService.Create: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            return await _roomRepository.DeleteRoom(id);
        }

        public async Task<List<RoomDto>> GetAll()
        {
            return _mapper.Map<List<RoomDto>>(await _roomRepository.GetAllWithRoomType());
        }

        public async Task<List<RoomDto>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int adults, int children)
        {
            var availableRooms = await _roomRepository.GetAvailableRooms(checkIn, checkOut);

            return _mapper.Map<List<RoomDto>>(availableRooms);
        }


        public async Task<Room> GetById(int id)
        {
            var room = await _roomRepository.GetById(id);
            return room;
        }
        public async Task<bool> Update(int id, Room room)
        {
            var existingRoom = await _roomRepository.GetById(id);
            if (existingRoom == null)
            {
                return false;
            }
            existingRoom.RoomCode = room.RoomCode;
            existingRoom.RoomStatus = room.RoomStatus;
            existingRoom.RoomTypeID = room.RoomTypeID;
            if (room.RoomImg != null && room.RoomImg.Length > 0)
            {
                existingRoom.RoomImg = room.RoomImg;
            }
            try
            {
                await _roomRepository.Update(id, existingRoom);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in RoomService.Update: " + ex.Message);
                return false;
            }
        }
    }
}
