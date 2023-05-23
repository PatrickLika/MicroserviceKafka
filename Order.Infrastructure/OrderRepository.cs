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

                _producer.Produce(_configuration["KafkaTopics:OrderReplyChannel"], 
                    new Message<string, string>
                    {
                        Key = orderEntity.Id,
                        Value = JsonConvert.SerializeObject(orderEntity)
                    },
                    deliveryReport =>
                    {
                        if (deliveryReport.Error.IsError)
                        {
                            Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                        }
                        else
                        {
                            Console.WriteLine($"Produced event to topic {deliveryReport.Topic}: key = {deliveryReport.Message.Key,-10} value = {deliveryReport.Message.Value}");
                        }
                    }
                );

                _producer.Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}
