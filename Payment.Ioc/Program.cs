using Confluent.Kafka;
using Payment.Application.Commands;
using Payment.Application.Commands.Implementation;
using Payment.Application.Repository;
using Payment.Infrastructure;
using Payment.Ioc;
using System;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IPaymentCreate, PaymentCreate>();
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
                GroupId = hostContext.Configuration["Kafka:PaymentGroup"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            return new ConsumerBuilder<string, string>(config).Build();
        });
        services.AddHostedService<PaymentConsumer>();
    })
    .Build();

host.Run();