using HomestayBooking.DTOs.BookingDto;
using HomestayBooking.Models;
using Microsoft.AspNetCore.Mvc;

namespace HomestayBooking.Repositories
{
    public interface IUserRepository : IBaseRepository<AppUser>
    {
        Task<bool> DeleteUser(string userId);

        Task<List<AppUser>> GetAllCustomers();
        Task<List<AppUser>> GetAllStaffs();

        Task<AppUser?> GetByEmail(string email);
        Task<bool> Update(AppUser user);
    }
}
