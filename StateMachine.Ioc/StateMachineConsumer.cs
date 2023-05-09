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

            while (true)
            {
                var message = _consumer.Consume();

                if (message != null)
                {
                    string jsonString = message.Message.Value;
                    Console.WriteLine(jsonString);

                    StateMachineDto dto = null;
                    try
                    {
                        dto = JsonConvert.DeserializeObject<StateMachineDto>(jsonString);
                    }
                    catch (JsonReaderException ex)
                    {
                        Console.WriteLine($"Error deserializing JSON string: {ex.Message}");
                    }


                    switch (dto.State)
                    {
                        case var state when state == States.OrderPending:
                            dto.StatePrevious = state;
                            dto.State = States.CustomerPending;
                            Console.WriteLine("Besked med ID: " + message.Message.Key + "Behandlet");
                            ProduceMessageAsync(_configuration["KafkaTopics:Customer"], message.Message.Key, dto);
                            break;

                        case var state when state == States.CustomerApproved && dto.StatePrevious == States.OrderPending:
                            dto.StatePrevious = state;
                            dto.State = States.StoragePending;
                            Console.WriteLine("Besked med ID: " + message.Message.Key + "Behandlet");
                            ProduceMessageAsync(_configuration["KafkaTopics:Storage"], message.Message.Key, dto);
                            break;

                        case var state when state == States.CustomerDenied:
                            dto.State = States.OrderDenied;
                            Console.WriteLine("Besked med ID: " + message.Message.Key + "Behandlet");
                            ProduceMessageAsync(_configuration["KafkaTopics:OrderReplyChannel"], message.Message.Key, dto);
                            break;

                        case var state when state == States.StorageApproved && dto.StatePrevious == States.CustomerApproved:
                            dto.StatePrevious = state;
                            dto.State = States.PaymentPending;
                            Console.WriteLine("Besked med ID: " + message.Message.Key + "Behandlet");
                            ProduceMessageAsync(_configuration["KafkaTopics:Payment"], message.Message.Key, dto);
                            break;

                        case var state when state == States.StorageDenied:
                            dto.State = States.OrderDenied;
                            Console.WriteLine("Besked med ID: " + message.Message.Key + "Behandlet");
                            ProduceMessageAsync(_configuration["KafkaTopics:OrderReplyChannel"], message.Message.Key, dto);
                            break;

                        case var state when state == States.PaymentApproved && dto.StatePrevious == States.StorageApproved:
                            dto.StatePrevious = state;
                            dto.State = States.ReceiptPending;
                            Console.WriteLine("Besked med ID: " + message.Message.Key + "Behandlet");
                            ProduceMessageAsync(_configuration["KafkaTopics:Receipt"], message.Message.Key, dto);
                            break;

                        case var state when state == States.PaymentDenied:
                            dto.State = States.Rollback;
                            Console.WriteLine("Besked med ID: " + message.Message.Key + "Behandlet");
                            ProduceMessageAsync(_configuration["KafkaTopics:Storage"], message.Message.Key, dto);
                            break;

                        case var state when state == States.ReceiptDone && dto.StatePrevious == States.PaymentApproved:
                            dto.StatePrevious = state;
                            dto.State = States.OrderApproved;
                            Console.WriteLine("Besked med ID: " + message.Message.Key + "Behandlet");
                            ProduceMessageAsync(_configuration["KafkaTopics:OrderReplyChannel"], message.Message.Key, dto);
                            break;

                        case var state when state == States.OrderApproved && dto.StatePrevious == States.ReceiptDone:
                            dto.State = States.OrderSuccessful;
                            Console.WriteLine("Besked med ID: " + message.Message.Key + "Behandlet");
                            ProduceMessageAsync(_configuration["KafkaTopics:OrderReplyChannel"], message.Message.Key, dto);
                            break;

                        case var state when state == States.OrderSuccessful:
                            Console.WriteLine("order færdig med id:" + message.Message.Key);
                            break;

                        case var state when state == States.OrderDenied:
                            Console.WriteLine("OrderDenied");
                            break;

                        default:
                            Console.WriteLine("Rallan vil gerne ha dansk: Ingen switch case fundet");
                            Console.WriteLine(message.Message.Value);
                            Console.WriteLine(message.Message.Key);
                            break;

                    }
                }
            }
            //Task.Run(() => Consume());
            //return Task.CompletedTask;
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            _consumer.Close();
            _consumer.Unsubscribe();
            return Task.CompletedTask;
        }
        private async Task Consume()
        {
            Console.WriteLine("Starting to consume messages");
            while (true)
            {
                //var message = _consumer.Consume(TimeSpan.FromSeconds(5));

                //if (message != null)
                //{

                //    var dto = JsonConvert.DeserializeObject<StateMachineDto>(message.Message.Value);

                //    switch (dto.State)
                //    {
                //        case var state when state == States.OrderPending:
                //            dto.StatePrevious = state;
                //            dto.State = States.CustomerPending;
                //            Console.WriteLine("Besked med ID: " + message.Message.Key + "Behandlet");
                //            await ProduceMessageAsync(_configuration["KafkaTopics:Customer"], message.Message.Key, dto);
                //            break;

                //        case var state when state == States.CustomerApproved && dto.StatePrevious == States.OrderPending:
                //            dto.StatePrevious = state;
                //            dto.State = States.StoragePending;
                //            Console.WriteLine("Besked med ID: " + message.Message.Key + "Behandlet");
                //            await ProduceMessageAsync(_configuration["KafkaTopics:Storage"], message.Message.Key, dto);
                //            break;

                //        case var state when state == States.CustomerDenied:
                //            dto.State = States.OrderDenied;
                //            Console.WriteLine("Besked med ID: " + message.Message.Key + "Behandlet");
                //            await ProduceMessageAsync(_configuration["KafkaTopics:OrderReplyChannel"], message.Message.Key, dto);
                //            break;

                //        case var state when state == States.StorageApproved && dto.StatePrevious == States.CustomerApproved:
                //            dto.StatePrevious = state;
                //            dto.State = States.PaymentPending;
                //            Console.WriteLine("Besked med ID: " + message.Message.Key + "Behandlet");
                //            await ProduceMessageAsync(_configuration["KafkaTopics:Payment"], message.Message.Key, dto);
                //            break;

                //        case var state when state == States.StorageDenied:
                //            dto.State = States.OrderDenied;
                //            Console.WriteLine("Besked med ID: " + message.Message.Key + "Behandlet");
                //            await ProduceMessageAsync(_configuration["KafkaTopics:OrderReplyChannel"], message.Message.Key, dto);
                //            break;

                //        case var state when state == States.PaymentApproved && dto.StatePrevious == States.StorageApproved:
                //            dto.StatePrevious = state;
                //            dto.State = States.ReceiptPending;
                //            Console.WriteLine("Besked med ID: " + message.Message.Key + "Behandlet");
                //            await ProduceMessageAsync(_configuration["KafkaTopics:Receipt"], message.Message.Key, dto);
                //            break;

                //        case var state when state == States.PaymentDenied:
                //            dto.State = States.Rollback;
                //            Console.WriteLine("Besked med ID: " + message.Message.Key + "Behandlet");
                //            await ProduceMessageAsync(_configuration["KafkaTopics:Storage"], message.Message.Key, dto);
                //            break;

                //        case var state when state == States.ReceiptDone && dto.StatePrevious == States.PaymentApproved:
                //            dto.StatePrevious = state;
                //            dto.State = States.OrderApproved;
                //            Console.WriteLine("Besked med ID: " + message.Message.Key + "Behandlet");
                //            await ProduceMessageAsync(_configuration["KafkaTopics:OrderReplyChannel"], message.Message.Key, dto);
                //            break;

                //        case var state when state == States.OrderApproved && dto.StatePrevious == States.ReceiptDone:
                //            dto.State = States.OrderSuccessful;
                //            Console.WriteLine("Besked med ID: " + message.Message.Key + "Behandlet");
                //            await ProduceMessageAsync(_configuration["KafkaTopics:OrderReplyChannel"], message.Message.Key, dto);
                //            break;

                //        case var state when state == States.OrderSuccessful:
                //            Console.WriteLine("order færdig med id:" + message.Message.Key);
                //            break;

                //        default:
                //            Console.WriteLine("Rallan vil gerne ha dansk: Ingen switch case fundet");
                //            Console.WriteLine(message.Message.Value);
                //            Console.WriteLine(message.Message.Key);
                //            break;

                //    }
                //}
            }
            Console.WriteLine("Stopped consuming messages");
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