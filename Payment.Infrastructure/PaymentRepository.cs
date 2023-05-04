using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payment.Application.Repository;
using Payment.Domain.Domain;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Payment.Infrastructure
{
    public class PaymentRepository : IPaymentRepository
    {


        private readonly IProducer<string, string> _producer;
        private readonly IConfiguration _configuration;

        public PaymentRepository(IProducer<string, string> producer, IConfiguration configuration)
        {
            _producer = producer;
            _configuration = configuration;
        }

        void IPaymentRepository.PaymentCreate(PaymentEntity paymentEntity)
        {
            try
            {
                _producer.ProduceAsync(_configuration["KafkaTopics:OrderReplyChannel"], new Message<string, string>
                {
                    Key = paymentEntity.Id,
                    Value = JsonConvert.SerializeObject(paymentEntity),
                });

                _producer.Flush();
            }
            catch (Exception e)
            {

            }
        }
    }
}
