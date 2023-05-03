using Microsoft.Extensions.Configuration;
using Storage.Domain.DomainService;

namespace Storage.Infrastructure.DomainService
{
    public class StorageDomainService : IStorageDomainService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public StorageDomainService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

    }
}
