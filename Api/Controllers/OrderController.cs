using Api.Dto;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

        public OrderController(IProducer<string, string> producer, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _producer = producer;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost]
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
        [HttpGet]
        public async Task<IActionResult> GetTable()
        {
            try
            {
                string query = "select * from PizzaCount;";

                var content = new StringContent($"{{ \"ksql\": \"{query}\", \"streamsProperties\": {{}} }}", Encoding.UTF8, "application/vnd.ksql.v1+json");
                HttpResponseMessage response = await _httpClient.PostAsync($"{_configuration["Kafka:KSqlDB"]}/query", content);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return Ok(result);
                }
                else
                {

                    return StatusCode((int)response.StatusCode, $"Error: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

    }
}
