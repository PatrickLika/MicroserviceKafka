using Confluent.Kafka;
using Newtonsoft.Json;
using Payment.Application.Commands;

namespace Payment.Ioc
{
    public class PaymentConsumer : IHostedService
    {

        private readonly IConsumer<string, string> _consumer;
        private readonly IPaymentCreate _paymentCreate;
        private readonly IConfiguration _configuration;


        public PaymentConsumer(IConsumer<string, string> consumer, IPaymentCreate paymentCreate, IConfiguration configuration)
        {
            _consumer = consumer;
            _paymentCreate = paymentCreate;
            _configuration = configuration;
        }

        Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(_configuration["KafkaTopics:Payment"]);
            Task.Run(() => Consume(cancellationToken));
            return Task.CompletedTask;
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            _consumer.Close();
            return Task.CompletedTask;
        }

        private async Task Consume(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = _consumer.Consume(TimeSpan.FromSeconds(5));

                if (message != null)
                {
                    var dto = JsonConvert.DeserializeObject<PaymentCreateDto>(message.Message.Value);
                    _paymentCreate.PaymentCreate(dto, message.Message.Key);
                }
            }
            _consumer.Close();
        }
    }


}
