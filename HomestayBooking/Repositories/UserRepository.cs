using HomestayBooking.Models;
using HomestayBooking.Models.DAL;
using Microsoft.EntityFrameworkCore;

namespace HomestayBooking.Repositories
{
    public class UserRepository : BaseRepository<AppUser>, IUserRepository
    {
        public UserRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public async Task<bool> DeleteUser(int userId)
        {
            return await DeleteAsync(userId);
        }

        public async Task<List<AppUser>> GetAllCustomers()
        {
            var customers = await _appDbContext.Users
            .Where(u => _appDbContext.UserRoles
            .Where(ur => _appDbContext.Roles
            .Where(r => r.Name == "Customer")
            .Select(r => r.Id)
            .Contains(ur.RoleId))
            .Select(ur => ur.UserId)
            .Contains(u.Id))
            .ToListAsync();

            return customers;
        }
    }

}
