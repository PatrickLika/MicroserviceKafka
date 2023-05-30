using Confluent.Kafka;
using Receipt.Application.Commands;
using Receipt.Application.Commands.Implementation;
using Receipt.Application.Repository;
using Receipt.Infrastructure;
using Receipt.Ioc;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddScoped<IReceiptRepository, ReceiptRepository>();
        services.AddScoped<IReceiptCreate, ReceiptCreate>();
        services.AddScoped<IProducer<string, string>>(provider =>
        {
            var config = new ProducerConfig
            {
                BootstrapServers = hostContext.Configuration["Kafka:BootstrapServers"]
            };
            return new ProducerBuilder<string, string>(config).Build();
        });

        services.AddScoped<IConsumer<string, string>>(provider =>
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = hostContext.Configuration["Kafka:BootstrapServers"],
                GroupId = hostContext.Configuration["Groups:ReceiptGroup"],
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };
            return new ConsumerBuilder<string, string>(config).Build();
        });
        services.AddHostedService<ReceiptConsumer>();
    })
    .Build();

host.Run();

