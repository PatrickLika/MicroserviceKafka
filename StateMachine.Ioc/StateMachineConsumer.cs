using Confluent.Kafka;
using Newtonsoft.Json;

namespace StateMachine.Ioc
{
    public class StateMachineConsumer : IHostedService
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly IProducer<string, string> _producer;
        private readonly IConfiguration _configuration;

        public StateMachineConsumer(IConsumer<string, string> consumer, IProducer<string, string> producer, IConfiguration configuration)
        {
            _consumer = consumer;
            _producer = producer;
            _configuration = configuration;
        }
        Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(_configuration["KafkaTopics:OrderReplyChannel"]);
            Task.Run(() => Consume(cancellationToken));

            return Task.CompletedTask;
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            _consumer.Close();
            _consumer.Unsubscribe();
            return Task.CompletedTask;
        }
        private async Task Consume(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = _consumer.Consume(cancellationToken);
                var dto = JsonConvert.DeserializeObject<StateMachineDto>(message.Message.Value);

                switch (dto.State)
                {
                    case var state when state == States.OrderPending:
                        dto.StatePrevious = state;
                        dto.State = States.CustomerPending;
                        await ProduceMessageAsync(_configuration["KafkaTopics:Customer"], message.Message.Key, dto);
                        break;

                    case var state when state == States.CustomerApproved && dto.StatePrevious == States.OrderPending:
                        dto.StatePrevious = state;    
                        dto.State = States.StoragePending;
                        await ProduceMessageAsync(_configuration["KafkaTopics:Storage"], message.Message.Key, dto);
                        break;

                    case var state when state == States.CustomerDenied:
                        dto.State = States.OrderDenied;
                        await ProduceMessageAsync(_configuration["KafkaTopics:OrderReplyChannel"], message.Message.Key, dto);
                        break;

                    case var state when state == States.StorageApproved && dto.StatePrevious == States.CustomerApproved:
                        dto.StatePrevious = state;
                        dto.State = States.PaymentPending;
                        await ProduceMessageAsync(_configuration["KafkaTopics:Payment"], message.Message.Key, dto);
                        break;

                    case var state when state == States.StorageDenied:
                        dto.State = States.OrderDenied;
                        await ProduceMessageAsync(_configuration["KafkaTopics:OrderReplyChannel"], message.Message.Key, dto);
                        break;

                    case var state when state == States.PaymentApproved && dto.StatePrevious == States.StorageApproved:
                        dto.StatePrevious = state;
                        dto.State = States.ReceiptPending;
                        await ProduceMessageAsync(_configuration["KafkaTopics:Receipt"], message.Message.Key, dto);
                        break;

                    case var state when state == States.PaymentDenied:
                        dto.State = States.Rollback;
                        await ProduceMessageAsync(_configuration["KafkaTopics:Storage"], message.Message.Key, dto);
                        break;

                    case var state when state == States.ReceiptDone && dto.StatePrevious == States.PaymentApproved:
                        dto.StatePrevious = state;
                        dto.State = States.OrderApproved;
                        await ProduceMessageAsync(_configuration["KafkaTopics:OrderReplyChannel"], message.Message.Key, dto);
                        break;

                    case var state when state == States.OrderApproved && dto.StatePrevious == States.ReceiptDone:
                        dto.State = States.OrderSuccessful;
                        await ProduceMessageAsync(_configuration["KafkaTopics:OrderReplyChannel"], message.Message.Key, dto);
                        break;

                    default:
                        Console.WriteLine("Rallan vil gerne ha dansk: Ingen switch case fundet");
                        break;
                }
            }
        }

        private async Task ProduceMessageAsync(string topic, string key, StateMachineDto dto)
        {
            await _producer.ProduceAsync(topic, new Message<string, string>
            {
                Key = key,
                Value = JsonConvert.SerializeObject(dto)
            });
            _producer.Flush();
        }

    }
}