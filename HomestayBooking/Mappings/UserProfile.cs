using AutoMapper;
using HomestayBooking.DTOs.AuthDto;
using HomestayBooking.Models;

namespace HomestayBooking.Mappings
{
    public class UserProfile: Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterDto, AppUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
        }
    }
}
