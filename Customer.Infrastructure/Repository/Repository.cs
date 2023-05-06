using Confluent.Kafka;
using Customer.Application.Repository;
using Customer.Domain.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Customer.Infrastructure.Repository
{
    public class Repository : IRepository
    {
        private readonly IProducer<string, string> _producer;
        private readonly IConfiguration _configuration;

        public Repository(IConfiguration configuration, IProducer<string, string> producer)
        {
            _configuration = configuration;
            _producer = producer;
        }

        async void IRepository.Produce(CustomerEntity entity)
        {
            if (entity.IsValid)
            {
                entity.State = States.CustomerApproved;
                await ProduceMessageAsync(entity);
            }
            else
            {
                entity.State = States.CustomerDenied;
                await ProduceMessageAsync(entity);
            }
        }

        private async Task ProduceMessageAsync(CustomerEntity entity)
        {
            await _producer.ProduceAsync(_configuration["KafkaTopics:OrderReplyChannel"], new Message<string, string>
            {
                Key = entity.Id,
                Value = JsonConvert.SerializeObject(entity)
            });
            _producer.Flush();
        }

    }

}