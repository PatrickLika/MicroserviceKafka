using Confluent.Kafka;
using Storage.Application.Commands;
using Storage.Application.Commands.Implementation;
using Storage.Application.Repository;
using Storage.Domain.DomainService;
using Storage.Infrastructure.DomainService;
using Storage.Infrastructure.Repository;
using Storage.Ioc;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddScoped<IStorageCommand, StorageCommand>();
        services.AddScoped<IStorageDomainService,StorageDomainService>();
        services.AddScoped<IRepository, Repository>();
        services.AddScoped<IRollBackStorage, RollbackStorage>();
        services.AddHttpClient();

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
                GroupId = hostContext.Configuration["Groups:StorageGroup"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            return new ConsumerBuilder<string, string>(config).Build();
        });

        services.AddHostedService<StorageConsumer>();

    })
    .Build();

host.Run();