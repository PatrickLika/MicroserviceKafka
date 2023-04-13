using Microsoft.Extensions.Configuration;
using Storage.Domain.DomainService;
using Storage.Domain.Model;
using System.Text;
using Newtonsoft.Json;

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

        async Task<StorageEntity> IStorageDomainService.GetStorageInformation()
        {

            string query = $"SELECT * FROM QUERYABLE_Storage;";
            var content = new StringContent($"{{ \"ksql\": \"{query}\", \"streamsProperties\": {{}} }}", Encoding.UTF8, "application/vnd.ksql.v1+json");
            HttpResponseMessage response = await _httpClient.PostAsync($"{_configuration["Kafka:KSqlDB"]}/query", content);
            var responseContent = await response.Content.ReadAsStringAsync();

           return JsonConvert.DeserializeObject<StorageEntity>(responseContent);

           //TODO Find ud af om dette skal sættes om en en wrapper
           
        }
    }
}
