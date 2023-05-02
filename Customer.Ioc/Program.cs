using Confluent.Kafka;
using Costumer.Application.Queries.Implementation;
using Costumer.Application.Queries;
using Costumer.Application.Repository;
using Costumer.Ioc;
using Customer.Domain.DomainService;
using Customer.Infrastructure.DomainService;
using Customer.Infrastructure.Repository;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddScoped<IRepository, Repository>();
        services.AddScoped<IReadCvr, ReadCvr>();
        services.AddScoped<ICustomerDomainService, CustomerDomainService>();

        services.AddScoped<IProducer<string, string>>(provider =>
        {
            var config = new ProducerConfig
            {
                BootstrapServers = provider.GetRequiredService<IConfiguration>()["Kafka:BootstrapServers"]
            };

            return new ProducerBuilder<string, string>(config).Build();
        });

        services.AddScoped<IConsumer<string, string>>(provider =>
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = provider.GetRequiredService<IConfiguration>()["Kafka:BootstrapServers"],
                GroupId = provider.GetRequiredService<IConfiguration>()["Groups:CustomerGroup"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            return new ConsumerBuilder<string, string>(config).Build();
        });


        services.AddHostedService<CostumerConsumer>();

    })
    .Build();

host.Run();