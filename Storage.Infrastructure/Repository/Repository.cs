using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Storage.Application.Commands;
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
                StorageDB(new StorageDbDto
                {
                    Id = "Storage",
                    Screws = entity.Screws,
                    Bolts = entity.Bolts,
                    Nails = entity.Nails
                });

                entity.State = States.StorageApproved;
            }
            else
            {
                entity.State = States.StorageDenied;
            }

            ProduceMessage(_configuration["KafkaTopics:OrderReplyChannel"], entity.Id, JsonConvert.SerializeObject(entity));

            _producer.Flush();
        }

        void IRepository.Rollback(StorageDbDto dto, StorageDto storageDto)
        {
            ProduceMessage(_configuration["KafkaTopics:StorageDB"], dto.Id, JsonConvert.SerializeObject(dto));

            storageDto.State = States.OrderDenied;
            ProduceMessage(_configuration["KafkaTopics:OrderReplyChannel"], dto.Id, JsonConvert.SerializeObject(storageDto) );

            _producer.Flush();
        }

        private void StorageDB(StorageDbDto dto)
        {
            //dto.Screws = -Math.Abs(dto.Screws);
            //dto.Bolts = -Math.Abs(dto.Bolts);
            //dto.Nails = -Math.Abs(dto.Nails);

            dto.Screws *= -1;
            dto.Bolts *= -1;
            dto.Nails *= -1;

            ProduceMessage(_configuration["KafkaTopics:StorageDB"],"Storage", JsonConvert.SerializeObject(dto));
        }

        private void ProduceMessage(string topic, string key, string value)
        {
            _producer.ProduceAsync(topic, new Message<string, string>
            {
                Key = key,
                Value = value
            }); 
        }
    }
}
