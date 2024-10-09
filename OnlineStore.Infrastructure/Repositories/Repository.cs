using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OnlineStore.Core.Interfaces;
using OnlineStore.Core.Models;
using OnlineStore.Infrastructure.Data;

namespace OnlineStore.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : DbModel
    {
        private readonly ApplicationContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T?> CreateNewAsync(T model)
        {
           EntityEntry<T> result = await _dbSet.AddAsync(model);
            
            await _context.SaveChangesAsync();

            return result.Entity;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(item => item.Id == id);
        }
    }
}
