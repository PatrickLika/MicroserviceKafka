using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Storage.Domain.DomainService;
using System.Text;

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

        bool IStorageDomainService.IsInStorage(int screws, int bolts, int nails)
        {
            string query = $"SELECT * FROM QUERYABLE_Storage;";
            var content = new StringContent($"{{ \"ksql\": \"{query}\", \"streamsProperties\": {{}} }}", Encoding.UTF8, "application/vnd.ksql.v1+json");
            HttpResponseMessage response = _httpClient.PostAsync($"{_configuration["Kafka:KSqlDB"]}/query", content).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            var payload = JsonConvert.DeserializeObject<PayloadWrapper>(responseContent);

            if (payload.Payload.Screws >= screws && payload.Payload.Bolts >= bolts && payload.Payload.Nails >= nails) return true;

            //TODO Find ud af om dette skal sættes om en en wrapper
            //

            return false;
        }

    }
}
