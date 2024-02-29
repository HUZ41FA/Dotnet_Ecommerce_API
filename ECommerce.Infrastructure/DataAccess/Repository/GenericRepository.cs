using ECommerce.Domain.Abstractions.IRepository;
using ECommerce.Domain.Models.Application;
using ECommerce.Infrastructure.DataAccess.ApplicationDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.DataAccess.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected ApplicationDBContext _context;
        protected DbSet<T> _dbSet;
        protected readonly ILogger _logger;

        public GenericRepository(ApplicationDBContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
            _dbSet = context.Set<T>();
        }
        public virtual async Task<T> GetById(string id)
        {
            return await _dbSet.FindAsync(id);
        }
        public virtual async Task<IEnumerable<T>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }
        public virtual async Task<bool> AddAsync(T type)
        {
            await _dbSet.AddAsync(type);
            return true;
        }
        public virtual async Task<bool> UpdateAsync(T type)
        {
            _dbSet.Update(type);
            return true;
        }
        public virtual async Task<bool> DeleteAsync(T type)
        {
            _dbSet.Remove(type);
            return true;
        }
    }
}
