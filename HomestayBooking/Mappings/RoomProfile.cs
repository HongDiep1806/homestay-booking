using AutoMapper;
using HomestayBooking.DTOs.RoomDto;
using HomestayBooking.Models;

namespace HomestayBooking.Mappings
{
    public class RoomProfile : Profile
    {
        public RoomProfile()
        {
            CreateMap<Room, RoomDto>()
                .ForMember(dest => dest.RoomType, opt => opt.MapFrom(src => src.RoomType.Name));
            CreateMap<RoomDto, Room>()
                .ForMember(dest => dest.RoomType, opt => opt.Ignore());

        }
    }
}
