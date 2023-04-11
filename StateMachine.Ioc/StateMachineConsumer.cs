using Confluent.Kafka;
using Newtonsoft.Json;
using System.Text;

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
            _consumer.Subscribe("OrderReplyChannel");
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

                //// KAFKA SYNTAX
                //string query = $"SELECT * FROM QUERYABLE_ORDERS WHERE OrderId = '{message.Message.Key}';";
                //var content = new StringContent($"{{ \"ksql\": \"{query}\", \"streamsProperties\": {{}} }}", Encoding.UTF8, "application/vnd.ksql.v1+json");
                //HttpResponseMessage response = await _httpClient.PostAsync($"{_configuration["Kafka:KSqlDB"]}/query", content);
                //var responseContent = await response.Content.ReadAsStringAsync();



                var dto = JsonConvert.DeserializeObject<StateMachineDto>(message.Message.Value);


                if (dto.State == "OrderPending")
                {
                    dto.State = "CostumerPending";


                    await _producer.ProduceAsync(_configuration["KafkaTopics:Customer"], new Message<string, string>
                    {
                        Key = message.Message.Key,
                        Value = JsonConvert.SerializeObject(dto)
                    });
                    _producer.Flush();
                }

                if (dto.State == "CostumerApproved")
                {
                    dto.State = "StoragePending";
                    await _producer.ProduceAsync(_configuration["KafkaTopics:Storage"], new Message<string, string>
                    {
                        Key = message.Message.Key,
                        Value = JsonConvert.SerializeObject(dto)
                    });
                    _producer.Flush();
                }

                if (dto.State == "StorageApproved")
                {
                    dto.State = "PaymentPending";
                    await _producer.ProduceAsync(_configuration["KafkaTopics:Payment"], new Message<string, string>
                    {
                        Key = message.Message.Key,
                        Value = JsonConvert.SerializeObject(dto)
                    });
                    _producer.Flush();
                }

                if (dto.State == "PaymentApproved")
                {
                    dto.State = "ReceiptPending";
                    await _producer.ProduceAsync(_configuration["KafkaTopics:Receipt"], new Message<string, string>
                    {
                        Key = message.Message.Key,
                        Value = JsonConvert.SerializeObject(dto)
                    });
                    _producer.Flush();
                }

                if (dto.State == "RecieptDone")
                {
                    dto.State = "OrderApproved";
                    await _producer.ProduceAsync(_configuration["KafkaTopics:OrderReplyChannel"], new Message<string, string>
                    {
                        Key = message.Message.Key,
                        Value = JsonConvert.SerializeObject(dto)
                    });
                    _producer.Flush();
                }

                if (dto.State == "OrderApproved")
                {
                    dto.State = "OrderSuccessful";

                    await _producer.ProduceAsync(_configuration["KafkaTopics:OrderReplyChannel"], new Message<string, string>
                    {
                        Key = message.Message.Key,
                        Value = JsonConvert.SerializeObject(dto)
                    });
                    _producer.Flush();
                }

            }

        }
    }
}