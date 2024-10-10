using Microsoft.EntityFrameworkCore;
using OnlineStore.Core.Interfaces;
using OnlineStore.Core.Models;
using OnlineStore.Core.Models.Dto;
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

        public async Task<bool> CreateNewPizza(PizzaDto pizzaDto)
        {
            Pizza pizza = new()
            {
                Category = pizzaDto.Category,
                Description = pizzaDto.Description,
                ImageUrl = pizzaDto.ImageUrl,
                Price = pizzaDto.Price,
                Title = pizzaDto.Title
            };

            await _context.Pizzas.AddAsync(pizza);
            return await _context.SaveChangesAsync() > 0;
        }

        public Task<Pizza?> GetPizzaByTitleAsync(string title)
        {
            return _context.Pizzas.FirstOrDefaultAsync(p => p.Title == title);
        }
    }
}
