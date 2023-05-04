using Api.Dto;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IProducer<string, string> _producer;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public OrderController(IProducer<string, string> producer, IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _producer = producer;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost("CreateOrder")]
        public ActionResult CreateOrder(OrderDto dto)
        {
            try
            {
                dto.State = "OrderPending";

                _producer.ProduceAsync(_configuration["KafkaTopics:OrderReplyChannel"], new Message<string, string>
                {
                    Key = Guid.NewGuid().ToString(),
                    Value = JsonConvert.SerializeObject(dto)
                });

                _producer.Flush();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPost("StorageDB")]
        public ActionResult StorageDB(StorageDbDto dto)
        {
            try
            {
                dto.Id = "Refill";
                _producer.ProduceAsync(_configuration["KafkaTopics:StorageDB"], new Message<string, string>
                {
                    Key = dto.Id,
                    Value = JsonConvert.SerializeObject(dto)
                });

                _producer.Flush();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetTable()
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

                return Ok(values);

        }
    }
}