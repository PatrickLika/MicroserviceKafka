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


        services.AddScoped<IConsumer<Guid, string>>(provider =>
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = hostContext.Configuration["Kafka:BootstrapServers"],
                GroupId = hostContext.Configuration["Groups:CustomerGroup"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            return new ConsumerBuilder<Guid, string>(config).Build();
        });


        services.AddHostedService<CostumerConsumer>();

    })
    .Build();

host.Run();