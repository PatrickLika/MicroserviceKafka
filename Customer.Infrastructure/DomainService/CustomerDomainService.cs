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
            _consumer.Subscribe(_iConfig["KafkaTopics:Cvr"]);
            _consumer.Assign(new TopicPartitionOffset(new TopicPartition(_iConfig["KafkaTopics:Cvr"], 0), Offset.Beginning));
            try
            {
                ConsumeResult<string, string> message;

                while ((message = _consumer.Consume()) != null && !message.IsPartitionEOF)
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