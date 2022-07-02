using ParkyWebSite.Models;
using ParkyWebSite.Repository.IRepository;
using System.Net.Http;

namespace ParkyWebSite.Repository
{

    public class TrailRepository : Repository<Trail>, ITrailRepository
    {
        private readonly IHttpClientFactory _clientFactory;
        public TrailRepository(IHttpClientFactory clientFactory) : base(clientFactory)
        {
            _clientFactory = clientFactory;
        }
    }
}
