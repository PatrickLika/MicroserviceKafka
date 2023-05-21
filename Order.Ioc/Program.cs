using Confluent.Kafka;
using Order.Application.Commands;
using Order.Application.Commands.Implementation;
using Order.Application.Repository;
using Order.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IRepository, OrderRepository>();
builder.Services.AddScoped<IOrderCreate, OrderCreate>();

builder.Services.AddScoped<IProducer<string, string>>(provider =>
{
    var config = new ProducerConfig
    {
        BootstrapServers = builder.Configuration.GetSection("Kafka")["BootstrapServers"]
    };
    return new ProducerBuilder<string, string>(config).Build();
});

var app = builder.Build();

app.Run();