using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Storage.Domain.DomainService;
using Storage.Domain.Model;
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

        async Task<bool> IStorageDomainService.IsInStorage(int screws, int bolts, int nails)
        {

            string query = $"SELECT * FROM QUERYABLE_Storage;";
            var content = new StringContent($"{{ \"ksql\": \"{query}\", \"streamsProperties\": {{}} }}", Encoding.UTF8, "application/vnd.ksql.v1+json");
            HttpResponseMessage response = await _httpClient.PostAsync($"{_configuration["Kafka:KSqlDB"]}/query", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            var payload = JsonConvert.DeserializeObject<PayloadWrapper>(responseContent);

            if (payload.Payload.Screws >= screws && payload.Payload.Bolts >= bolts && payload.Payload.Nails >= nails) return true;

            else return false;

        }

        private class PayloadWrapper
        {
            public StorageEntity Payload { get; set; }
        }
    }
}
