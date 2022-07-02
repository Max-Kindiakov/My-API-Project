using System.Collections.Generic;
using System.Threading.Tasks;

namespace ParkyWebSite.Repository.IRepository
{
    public interface IRepository<T> where T : class //generic T class (T може бути тільки класом)
    {
        Task<T> GetAsync(string url, int Id, string token);
        Task<IEnumerable<T>> GetAllAsync(string url, string token);
        Task<bool> CreateAsync(string url, T objToCreate, string token);
        Task<bool> UpdateAsync(string url, T objToUpdate, string token);
        Task<bool> DeleteAsync(string url, int Id, string token);
    }
}
