namespace HomestayBooking.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> GetById(int id);
        Task<List<T>> GetAll();
        Task Create(T entity);
        Task<bool> Update(int id, T entity);
        Task<bool> DeleteAsync(int id);
        
        //Task<bool> AddRanges(List<T> ranges);
        //Task<int> Count();
    }
}
