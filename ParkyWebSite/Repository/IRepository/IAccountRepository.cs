using ParkyWebSite.Models;
using System.Threading.Tasks;

namespace ParkyWebSite.Repository.IRepository
{
    public interface IAccountRepository:IRepository<User>
    {
        Task<User> LoginAsync(string url, User objToCreate); //url to api call; objToCreate wit login and password
        Task<bool> RegisterAsync(string url, User objToCreate);
    }
}
