using Confluent.Kafka;
using Costumer.Application.Queries.Implementation;
using Costumer.Application.Queries;
using Costumer.Application.Repository;
using Costumer.Ioc;
using Costumer.Infrastructure;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddScoped<IRepository, Repository>();
        services.AddScoped<IReadCvr, ReadCvr>();

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
                GroupId = hostContext.Configuration["Groups:CustomerGroup"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            return new ConsumerBuilder<string, string>(config).Build();
        });


        services.AddHostedService<CostumerConsumer>();

    })
    .Build();

host.Run();