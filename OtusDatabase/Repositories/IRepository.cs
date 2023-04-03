namespace OtusDatabase.Repositories
{
    public interface IRepository<T> where T : IEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetAsync(long id);
        Task<long> AddAsync(T entity);
        Task<int> UpdateAsync(T entity);
        Task<bool> DeleteAsync(long id);
    }
}
