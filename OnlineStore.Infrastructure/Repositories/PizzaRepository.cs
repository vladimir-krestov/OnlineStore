using Microsoft.EntityFrameworkCore;
using OnlineStore.Core.Interfaces;
using OnlineStore.Core.Models;
using OnlineStore.Infrastructure.Data;

namespace OnlineStore.Infrastructure.Repositories
{
    public class PizzaRepository : Repository<Pizza>, IPizzaRepository
    {
        private readonly ApplicationContext _context;

        public PizzaRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Pizza?> GetByIdAsync(int id)
        {
            return await _context.Pizzas.FirstOrDefaultAsync(item => item.Id == id);
        }
    }
}
