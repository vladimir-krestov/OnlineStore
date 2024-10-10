using OnlineStore.Core.Models;
using OnlineStore.Core.Models.Dto;

namespace OnlineStore.Core.Interfaces
{
    public interface IPizzaRepository : IRepository<Pizza>
    {
        Task<bool> CreateNewPizza(PizzaDto pizzaDto);
    }
}
