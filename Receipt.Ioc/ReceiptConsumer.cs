using Confluent.Kafka;
using Newtonsoft.Json;
using Receipt.Application.Commands;

namespace Receipt.Ioc
{
    public class ReceiptConsumer : IHostedService
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly IReceiptCreate _receiptCreate;

        public ReceiptConsumer(IConsumer<string, string> consumer, IReceiptCreate receiptCreate)
        {
            _consumer = consumer;
            _receiptCreate = receiptCreate;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _consumer.Subscribe("Receipt");
            Task.Run(() => Consume(cancellationToken));
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _consumer.Close();
            _consumer.Unsubscribe();
        }

        private async Task Consume(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = _consumer.Consume(TimeSpan.FromSeconds(5));
                if (message != null)
                {
                    var dto = JsonConvert.DeserializeObject<ReceiptCreateDto>(message.Message.Value);
                    await _receiptCreate.ReceiptCreate(dto, message.Message.Key);
                }
            }
            _consumer.Close();
        }
    }
}
