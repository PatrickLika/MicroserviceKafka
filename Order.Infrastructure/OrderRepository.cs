using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Order.Application.Repository;
using Order.Domain.Domain;

namespace Order.Infrastructure
{
    public class OrderRepository : IRepository
    {
        private readonly IProducer<string, string> _producer;
        private readonly IConfiguration _configuration;

        public OrderRepository(IProducer<string, string> producer, IConfiguration configuration)
        {
            _producer = producer;
            _configuration = configuration;
        }


        public void CreateOrder(OrderEntity orderEntity)
        {
            try
            {
                orderEntity.State = States.OrderPending;

                _producer.ProduceAsync(_configuration["KafkaTopics:OrderReplyChannel"], new Message<string, string>
                {
                    Key = Guid.NewGuid().ToString(),
                    Value = JsonConvert.SerializeObject(orderEntity)
                });

                _producer.Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}
