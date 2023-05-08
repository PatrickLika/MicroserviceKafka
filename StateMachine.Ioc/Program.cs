using Confluent.Kafka;
using StateMachine.Ioc;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
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
                GroupId = hostContext.Configuration["Groups:OrderReplyChannelGroup"],
                AutoOffsetReset = AutoOffsetReset.Earliest,
                MaxPollIntervalMs = 20000,
                SessionTimeoutMs = 10000,
                Debug = "consumer,cgrp,topic,fetch"
            };

            return new ConsumerBuilder<string, string>(config)
                .SetPartitionsAssignedHandler((_, partitions) =>
                {
                    Console.WriteLine($"StateMachineConsumer: Assigned partitions: [{string.Join(", ", partitions)}]");
                })
                .SetPartitionsRevokedHandler((_, partitions) =>
                {
                    Console.WriteLine($"StateMachineConsumer: Revoked partitions: [{string.Join(", ", partitions)}]");
                })
                .Build();
        });

        services.AddHostedService<StateMachineConsumer>();

    })
    .Build();

host.Run();