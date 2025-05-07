
using HomestayBooking.Models.DAL;
using Microsoft.EntityFrameworkCore;

namespace HomestayBooking.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly AppDbContext _appDbContext;
        public BaseRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;  
        }
        public Task<bool> AddRanges(List<T> ranges)
        {
            throw new NotImplementedException();
        }

        public async Task Create(T entity)
        {
            await _appDbContext.Set<T>().AddAsync(entity);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<List<T>> GetAll()
        {
            return await _appDbContext.Set<T>().ToListAsync();  
        }

        public async Task<T> GetById(int id)
        {
            return await _appDbContext.Set<T>().FindAsync(id);
        }

        public async Task<bool> Update(int id, T entity)
        {
            var existingEntity = await _appDbContext.Set<T>().FindAsync(id);
            if (existingEntity == null)
            {
                return false;
            }

            _appDbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
            await _appDbContext.SaveChangesAsync();

            return true;
        }
    }
}
