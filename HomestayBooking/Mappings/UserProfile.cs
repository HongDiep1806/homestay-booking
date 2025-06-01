using AutoMapper;
using HomestayBooking.DTOs.AuthDto;
using HomestayBooking.DTOs.UserDto;
using HomestayBooking.Models;

namespace HomestayBooking.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterDto, AppUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
            CreateMap<AppUser, UserDto>()
           .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
           .ForMember(dest => dest.Password, opt => opt.Ignore());

            CreateMap<UserDto, AppUser>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => (bool?)src.IsActive));

            CreateMap<AppUser, UserDto>().ReverseMap();

            CreateMap<AppUser, UserDto>().ReverseMap()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
        }
    }
}
