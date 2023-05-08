using Confluent.Kafka;
using Newtonsoft.Json;
using Storage.Application.Commands;

namespace Storage.Ioc
{
    public class StorageConsumer : IHostedService
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly IConfiguration _configuration;
        private readonly IStorageCommand _storageCommand;
        private readonly IRollBackStorage _rollBackStorage;

        public StorageConsumer(IConfiguration configuration, IConsumer<string, string> consumer, IStorageCommand storageCommand, IRollBackStorage rollBackStorage)
        {
            _configuration = configuration;
            _consumer = consumer;
            _storageCommand = storageCommand;
            _rollBackStorage = rollBackStorage;
        }

        Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(_configuration["KafkaTopics:Storage"]);
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
                    var dto = JsonConvert.DeserializeObject<StorageDto>(message.Message.Value);
                    dto.Id = message.Message.Key;

                    if (dto.State == States.StoragePending)
                    {
                        dto.Id = message.Message.Key;
                        _storageCommand.CheckStorage(dto);
                    }

                    else if(dto.State == States.Rollback)
                    {
                        _rollBackStorage.RollBackStorage(new StorageDbDto
                        {
                            Id = "Storage",
                            Screws = dto.Screws,
                            Bolts = dto.Bolts,
                            Nails = dto.Nails
                        });
                    }

                    else
                    {
                        Console.WriteLine("State fejl i storage");
                    }
                }
            }
            _consumer.Close();
        }
    }
}
