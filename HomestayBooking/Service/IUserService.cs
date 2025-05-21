using HomestayBooking.DTOs.UserDto;
using HomestayBooking.Models;

namespace HomestayBooking.Service
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllCustomers();
        Task<bool> Update(int id, AppUser user);
        Task<bool> Delete(int id);
        Task<AppUser> GetById(int id);
    }
}
