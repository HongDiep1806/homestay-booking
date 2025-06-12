
using HomestayBooking.Models;
using HomestayBooking.Models.DAL;
using Microsoft.EntityFrameworkCore;

namespace HomestayBooking.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class, BaseEntity
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
    try
    {
                Console.WriteLine($"Attempting to add {typeof(T).Name} to the database...");
                await _appDbContext.Set<T>().AddAsync(entity);
        var changes = await _appDbContext.SaveChangesAsync();
        Console.WriteLine($"Successfully added {typeof(T).Name} to the database. Number of records saved: {changes}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error adding {typeof(T).Name} to the database: {ex.Message}");
    }
}



        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _appDbContext.Set<T>().FindAsync(id);
            if (item == null)
                return false;

            item.IsDeleted = true;
            var result = await Update(id, item);
            Console.WriteLine("Deleted item: " + result );  
            return true;
        }


        public async Task<List<T>> GetAll()
        {
            var items =  await _appDbContext.Set<T>().ToListAsync();
            return items.Where(i => i.IsDeleted == false).ToList();
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
