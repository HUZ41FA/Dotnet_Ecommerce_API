

namespace ECommerce.Domain.Abstractions.IRepository
{
    public interface IGenericRepository<T> where T : class 
    {
        Task<T> GetById(string id);
        Task<IEnumerable<T>> GetAll();
        Task<bool> AddAsync(T type);
        Task<bool> UpdateAsync(T type);
        Task<bool> DeleteAsync(T Type);
    }
}
