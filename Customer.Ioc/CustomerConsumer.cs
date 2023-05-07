using Confluent.Kafka;
using Customer.Application.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using Customer.Ioc;

namespace Costumer.Ioc
{
    public class CustomerConsumer : IHostedService
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly IReadCvr _iReadCvr;
        private readonly IConfiguration _configuration;

        public CustomerConsumer(CustomerConsumerWrapper customerConsumerWrapper, IReadCvr iReadCvr, IConfiguration configuration)
        {
            _consumer = customerConsumerWrapper.Consumer;
            _iReadCvr = iReadCvr;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(_configuration["KafkaTopics:Customer"]);
            Task.Run(() => Consume(cancellationToken));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _consumer.Close();
            return Task.CompletedTask;
        }

        private async Task Consume(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = _consumer.Consume(cancellationToken);
                var dto = JsonConvert.DeserializeObject<ReadCvrDto>(message.Message.Value);
                dto.Id = message.Message.Key;
                _iReadCvr.ReadCvr(dto);
            }
            _consumer.Close();
        }
    }
}