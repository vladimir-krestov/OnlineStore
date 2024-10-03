namespace OnlineStore.Core.Interfaces
{
    public interface IRepository<T> where T : class 
    {
        Task<IEnumerable<T>> GetAllAsync();
    }
}
