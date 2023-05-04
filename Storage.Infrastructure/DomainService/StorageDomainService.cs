using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        StorageDbDto IStorageDomainService.GetStorage()
        {
            string query = $"select * from REMAININGSTORAGE;";

            var queryRequest = new
            {
                ksql = query,
                streamsProperties = new { }
            };

            var content = new StringContent(JsonConvert.SerializeObject(queryRequest), Encoding.UTF8, "application/vnd.ksql.v1+json");
            HttpResponseMessage response = _httpClient.PostAsync($"{_configuration["Kafka:KSqlDB"]}/query", content).Result;
            string result = response.Content.ReadAsStringAsync().Result;

            JArray jsonResponse = JArray.Parse(result);
            JObject rowData = (JObject)jsonResponse.FirstOrDefault(x => x["row"] != null);
            JArray row = rowData != null ? (JArray)rowData["row"]["columns"] : null;

            StorageDbDto values = new StorageDbDto
            {
                Id = row[0].ToObject<string>(),
                Screws = row[1].ToObject<int>(),
                Bolts = row[2].ToObject<int>(),
                Nails = row[3].ToObject<int>()
            };

            return values;
        }
    }
}
