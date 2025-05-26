using HomestayBooking.DTOs.UserDto;
using HomestayBooking.Models;

namespace HomestayBooking.Service
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllCustomers();
        Task<List<UserDto>> GetAllStaffs();
        Task<bool> UpdateByEmail(string email, AppUser user);
        Task<bool> Delete(string id);
        Task<AppUser> GetByEmail(string email);
    }
}
