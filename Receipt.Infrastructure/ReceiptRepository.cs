using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Receipt._Domain.Model;
using Receipt.Application.Repository;

namespace Receipt.Infrastructure
{
    public class ReceiptRepository : IReceiptRepository
    {
        private readonly IProducer<string, string> _producer;
        private readonly IConfiguration _configuration;
        public ReceiptRepository(IProducer<string, string> producer, IConfiguration configuration)
        {
            _producer = producer;
            _configuration = configuration;
        }


        async Task IReceiptRepository.CreateReceipt(ReceiptEntity receiptEntity)
        {
            Console.WriteLine($"Kvittering er blevet sendt. Kunde CVR: {receiptEntity.Cvr}");
            receiptEntity.State = States.ReceiptDone;

            try
            {
                await _producer.ProduceAsync(_configuration["KafkaTopics:OrderReplyChannel"], new Message<string, string>
                {
                    Key = receiptEntity.Id,
                    Value = JsonConvert.SerializeObject(receiptEntity),
                });

                _producer.Flush();

            }
            catch (Exception e)
            {

            }

        }
    }
}
