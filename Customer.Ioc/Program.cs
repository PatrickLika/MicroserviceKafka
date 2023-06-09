using Confluent.Kafka;
using Customer.Application.Queries;
using Customer.Application.Queries.Implementation;
using Customer.Application.Repository;
using Customer.Domain.DomainService;
using Customer.Infrastructure.DomainService;
using Customer.Infrastructure.Repository;
using Customer.Ioc;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddScoped<IRepository, Repository>();
        services.AddScoped<IReadCvr, ReadCvr>();
        services.AddScoped<ICustomerDomainService, CustomerDomainService>();

        services.AddScoped<IProducer<string, string>>(provider =>
        {
            var config = new ProducerConfig
            {
                BootstrapServers = hostContext.Configuration["Kafka:BootstrapServers"]
            };

            return new ProducerBuilder<string, string>(config).Build();
        });

        services.AddScoped<CustomerConsumerWrapper>(provider =>
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = hostContext.Configuration["Kafka:BootstrapServers"],
                GroupId = hostContext.Configuration["Groups:CustomerGroup"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            return new CustomerConsumerWrapper(new ConsumerBuilder<string, string>(config).Build());
        });


        services.AddScoped<IConsumer<string,string>>(provider =>
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = hostContext.Configuration["Kafka:BootstrapServers"],
                GroupId = hostContext.Configuration["Groups:CvrGroup"],
            };

            return new ConsumerBuilder<string, string>(config).Build();
        });

        services.AddHostedService<CustomerConsumer>();
    })
    .Build();

host.Run();