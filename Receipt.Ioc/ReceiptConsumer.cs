using Confluent.Kafka;
using Newtonsoft.Json;
using Receipt.Application.Commands;

namespace Receipt.Ioc
{
    public class ReceiptConsumer: IHostedService
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly IReceiptCreate _receiptCreate;
        private Task _executingTask;

        public ReceiptConsumer(IConsumer<string, string> consumer, IReceiptCreate receiptCreate)
        {
            _consumer=consumer;
            _receiptCreate=receiptCreate;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _consumer.Subscribe("Receipt");
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
                var dto = JsonConvert.DeserializeObject<ReceiptCreateDto>(message.Message.Value);
                _receiptCreate.ReceiptCreate(dto, message.Message.Key);
            }
            _consumer.Close();
        }
    }
}
