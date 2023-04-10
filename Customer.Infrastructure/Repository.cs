using Confluent.Kafka;
using Costumer.Application.Repository;
using Customer.Application.Queries;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Costumer.Infrastructure
{
    public class Repository : IRepository
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly IConfiguration _configuration;
        private readonly IProducer<string, string> _producer;

        public Repository(IConfiguration configuration)
        {
            _configuration = configuration;

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                GroupId = _configuration["Groups:CvrGroup"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        }

        void IRepository.ReadCvr(ReadCvrDto dto)
        {
            string topic = _configuration["KafkaTopics:Cvr"];
            var partition = new TopicPartition(topic, 0);
            _consumer.Assign(new TopicPartitionOffset(partition, Offset.Beginning));

            while (true)
            {
                var message = _consumer.Consume();

                if (message == null)
                {
                    dto.State = "CvrDenied";
                    _producer.ProduceAsync(_configuration["KafkaTopics:OrderReplyChannel"], new Message<string, string>
                    {
                        Key = dto.Id,
                        Value = JsonConvert.SerializeObject(dto)
                    });
                    break;
                }

                var payloadWrapper = JsonConvert.DeserializeObject<PayloadWrapper>(message.Message.Value);


                if (dto.Cvr == payloadWrapper.Payload.Cvr)
                {
                    dto.State = "CvrApproved";
                    _producer.ProduceAsync(_configuration["KafkaTopics:OrderReplyChannel"], new Message<string, string>
                    {
                        Key = dto.Id,
                        Value = JsonConvert.SerializeObject(dto)
                    });
                    break;
                }
            }
        }


    }
}