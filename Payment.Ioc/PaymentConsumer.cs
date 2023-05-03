using Confluent.Kafka;
using Newtonsoft.Json;
using Payment.Application.Commands;

namespace Payment.Ioc
{
    public class PaymentConsumer : IHostedService
    {

        private readonly IConsumer<string, string> _consumer;
        private readonly IPaymentCreate _paymentCreate;
        private Task _executingTask;


        public PaymentConsumer(IConsumer<string, string> consumer, IPaymentCreate paymentCreate)
        {
            _consumer = consumer;
            _paymentCreate = paymentCreate;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _consumer.Subscribe("Payment");
            _executingTask = Task.Run(() => Consume(cancellationToken));
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask != null)
            {
                _consumer.Close();
                await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));
            }
        }

        private async Task Consume(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = _consumer.Consume(cancellationToken);
                var dto = JsonConvert.DeserializeObject<PaymentCreateDto>(message.Message.Value);
                _paymentCreate.PaymentCreate(dto, message.Message.Key); //
            }
            _consumer.Close();
        }
    }


}
