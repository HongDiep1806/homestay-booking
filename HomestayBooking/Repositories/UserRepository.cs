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

        public async Task<bool> DeleteUser(string userId)
        {
            var user = await _appDbContext.Users.FindAsync(userId);
            if (user == null)
            {
                Console.WriteLine("User not found"); 
                return false;
            }
            user.IsDeleted = true;
            _appDbContext.Users.Update(user);
            await _appDbContext.SaveChangesAsync();
            Console.WriteLine("Deleted successfully");
            return true;
        }

        public async Task<List<AppUser>> GetAllCustomers()
        {
            var customers = await _appDbContext.Users
            .Where(u => !u.IsDeleted && _appDbContext.UserRoles
            .Where(ur => _appDbContext.Roles
            .Where(r => r.Name == "Customer")
            .Select(r => r.Id)
            .Contains(ur.RoleId))
            .Select(ur => ur.UserId)
            .Contains(u.Id))
            .ToListAsync();

            return customers;
        }

        public async Task<List<AppUser>> GetAllStaffs()
        {
            var staffs = await _appDbContext.Users
            .Where(u => !u.IsDeleted && _appDbContext.UserRoles
            .Where(ur => _appDbContext.Roles
            .Where(r => r.Name == "Staff")
            .Select(r => r.Id)
            .Contains(ur.RoleId))
            .Select(ur => ur.UserId)
            .Contains(u.Id))
            .ToListAsync();

            return staffs;
        }

        public async Task<AppUser?> GetByEmail(string email)
        {
            return await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> Update(AppUser user)
        {
            _appDbContext.Users.Update(user);
            await _appDbContext.SaveChangesAsync();
            return true;
        }
    }

}
