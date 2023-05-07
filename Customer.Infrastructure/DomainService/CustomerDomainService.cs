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


        public CustomerDomainService(IConsumer<string, string> consumer, IConfiguration iConfig)
        {
            _consumer = consumer;
            _iConfig = iConfig;
        }

        bool ICustomerDomainService.CvrIsValid(string cvr)
        {
            _consumer.Assign(new TopicPartitionOffset(new TopicPartition(_iConfig["KafkaTopics:Cvr"], 0), Offset.Beginning));

            try
            {
                while (true)
                {
                    var message = _consumer.Consume(TimeSpan.FromSeconds(5));
                    if (message == null || message.IsPartitionEOF) return false;

                    var payloadWrapper = JsonConvert.DeserializeObject<PayloadWrapper>(message.Message.Value);
                    if (payloadWrapper.Payload.Cvr == cvr) return true;
                }
            }
            catch (OperationCanceledException)
            {
                _consumer.Close();
                return false;
            }

        }


    }
}