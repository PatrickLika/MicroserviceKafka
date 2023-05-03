using System.Runtime.Serialization;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Storage.Application.Repository;
using Storage.Domain.Model;

namespace Storage.Infrastructure.Repository
{
    public class Repository : IRepository
    {
        private readonly IProducer<string, string> _producer;
        private readonly IConfiguration _configuration;
        public Repository(IProducer<string, string> producer, IConfiguration configuration)
        {
            _producer = producer;
            _configuration = configuration;
        }
        void IRepository.Produce(StorageEntity entity)
        {
            if (entity.IsInStorage)
            {
                entity.State = States.CustomerApproved;
            }
            else
            {
                entity.State = States.CustomerDenied;
            }


            entity.State = States.StorageApproved;

            _producer.ProduceAsync(_configuration["KafkaTopics:OrderReplyChannel"], new Message<string, string>
            {
                Key = entity.Id,
                Value = JsonConvert.SerializeObject(entity)
            });
            _producer.Flush();
        }


    }
}
