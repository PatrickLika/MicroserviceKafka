using Confluent.Kafka;
using Customer.Domain.DomainService;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Customer.Infrastructure.DomainService
{
    public class CustomerDomainService : ICustomerDomainService
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly IConfiguration _iConfig;

        public CustomerDomainService(IConfiguration iConfiguration)
        {
            _iConfig = iConfiguration;
            var config = new ConsumerConfig
            {
                BootstrapServers = _iConfig["Kafka:BootstrapServers"],
                GroupId = _iConfig["Groups:CvrGroup"],
            };

            _consumer = new ConsumerBuilder<string, string>(config).Build();
            _consumer.Assign(new TopicPartitionOffset(new TopicPartition(_iConfig["KafkaTopics:Cvr"], 0), Offset.Beginning));
        }

        bool ICustomerDomainService.CvrIsValid(string cvr)
        {
            try
            {
                ConsumeResult<string, string> message;

                while ((message = _consumer.Consume(TimeSpan.FromSeconds(5))) != null && !message.IsPartitionEOF)
                {
                    var payloadWrapper = JsonConvert.DeserializeObject<PayloadWrapper>(message.Message.Value);

                    if (payloadWrapper.Payload.Cvr == cvr)
                    {
                        return true;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _consumer.Close();
            }

            return false;
        }

    }
}