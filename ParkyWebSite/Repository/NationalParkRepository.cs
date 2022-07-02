using ParkyWebSite.Models;
using ParkyWebSite.Repository.IRepository;
using System.Net.Http;

namespace ParkyWebSite.Repository
{//метод, за допомогою якого дістаємось до парк репозиторію (в репозиторії функціонал)
    public class NationalParkRepository : Repository<NationalPark>, INationalParkRepository
    {
        private readonly IHttpClientFactory _clientFactory;
        public NationalParkRepository(IHttpClientFactory clientFactory) : base(clientFactory)
        {
            _clientFactory = clientFactory;
        }
    }
}
