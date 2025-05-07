using AutoMapper;
using HomestayBooking.DTOs.RoomDto;
using HomestayBooking.Models;

namespace HomestayBooking.Mappings
{
    public class RoomProfile : Profile
    {
        public RoomProfile()
        {
            CreateMap<Room, RoomDto>();
            
        }
    }
}
