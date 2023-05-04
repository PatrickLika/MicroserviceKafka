using Confluent.Kafka;
using Newtonsoft.Json;
using Storage.Application.Commands;
using Storage.Application.Commands.Implementation;

namespace Storage.Ioc
{
    public class StorageConsumer : IHostedService
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly IConfiguration _configuration;
        private readonly IStorageCommand _storageCommand;

        public StorageConsumer(IConfiguration configuration, IConsumer<string, string> consumer, IStorageCommand storageCommand)
        {
            _configuration = configuration;
            _consumer = consumer;
            _storageCommand = storageCommand;
        }

        async Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(_configuration["KafkaTopics:Storage"]);

            while (!cancellationToken.IsCancellationRequested)
            {
                var message = _consumer.Consume(cancellationToken);
                var dto = JsonConvert.DeserializeObject<StorageDto>(message.Message.Value);
                dto.Id = message.Message.Key;

                _storageCommand.CheckStorage(dto);
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
