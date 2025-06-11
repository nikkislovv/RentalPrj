using CatalogApi.Logger;
using Domain.Infrastructure;
using Domain.Infrastructure.Repositories;
using Domain.Infrastructure.Services;
using Infrastructure.Handlers;
using Infrastructure.Producers;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Stores;
using MediatR;
using MongoDB.Bson.Serialization;
using RentCommandApi.Events;
using RentCommandApi.Extensions;
using RentCommandApi.Logger;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", false, true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                        false, true)
                    .Build();

LoggerConfigurator.ConfigureLogging(configuration);
builder.Host.UseSerilog();

BsonClassMap.RegisterClassMap<BaseEvent>();
BsonClassMap.RegisterClassMap<RentCreatedEvent>();
BsonClassMap.RegisterClassMap<RentDeletedEvent>();
BsonClassMap.RegisterClassMap<RentExtendedEvent>();
BsonClassMap.RegisterClassMap<RentFinishedEvent>();
BsonClassMap.RegisterClassMap<RentStartedEvent>();

// Add services to the container.

services
    .ConfigureJWT(builder.Configuration)
    .ConfigureMongoDBHealthChecker(builder.Configuration)
    .ConfigureVersioning()
    .AddSingleton<ILoggerManager, LoggerManager>()
    .ConfigureMongoDb(builder.Configuration)
    .AddScoped<IEventRepository, EventRepository>()
    .AddScoped<IEventStore, EventStore>()
    .AddScoped<IEventSourcingHandler, EventSourcingHandler>()
    .AddMediatR(typeof(Program).Assembly)
    .ConfigureElasticsearchStorage(builder.Configuration)
    .AddScoped<ICarElasticsearchService, CarElasticsearchService>()
    .ConfigureKafkaProducer(builder.Configuration)
    .AddScoped<IEventProducer,EventProducer>()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddControllers();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

