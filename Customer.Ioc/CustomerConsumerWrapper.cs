using Confluent.Kafka;

namespace Customer.Ioc
{
    public class CustomerConsumerWrapper
    {
        public IConsumer<string, string> Consumer { get; }

        public CustomerConsumerWrapper(IConsumer<string, string> consumer)
        {
            Consumer = consumer;
        }
    }
}

