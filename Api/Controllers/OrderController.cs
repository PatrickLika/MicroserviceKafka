using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Order.Application.Commands;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IProducer<string, string> _producer;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IOrderCreate _orderCreate;

        public OrderController(IProducer<string, string> producer, IConfiguration configuration,
            IHttpClientFactory httpClientFactory, IOrderCreate orderCreate)
        {
            _producer = producer;
            _configuration = configuration;
            _orderCreate = orderCreate;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost("CreateOrder")]
        public ActionResult CreateOrder(OrderDto dto)
        {
            try
            {
                _orderCreate.Create(dto);
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpPost("StorageDB")]
        public ActionResult StorageDB(StorageDbDto dto)
        {
            try
            {
                dto.Id = "Storage";
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

    }
}