using Confluent.Kafka;
using Costumer.Application.Queries;
using Customer.Application.Queries;
using Newtonsoft.Json;

namespace Costumer.Ioc
{
    public class CostumerConsumer : IHostedService
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly IReadCvr _iReadCvr;
        private readonly IConfiguration _configuration;

        public CostumerConsumer(IConsumer<string, string> consumer, IReadCvr iReadCvr)
        {
            _consumer = consumer;
            _iReadCvr = iReadCvr;
        }

        async Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            _consumer.Subscribe("Customer");

            while (!cancellationToken.IsCancellationRequested)
            {
                var message = _consumer.Consume(cancellationToken);
                var dto = JsonConvert.DeserializeObject<ReadCvrDto>(message.Message.Value);
                dto.Id = message.Message.Key;

                _iReadCvr.ReadCvr(dto);
            }

            await Task.CompletedTask;
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            _consumer.Close();
            return Task.CompletedTask;
        }
    }
}