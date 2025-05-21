using HomestayBooking.Models;

namespace HomestayBooking.Repositories
{
    public interface IUserRepository : IBaseRepository<AppUser>
    {
        Task<bool> DeleteUser(int userId);

        Task<List<AppUser>> GetAllCustomers();
    }
}
