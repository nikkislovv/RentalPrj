using Confluent.Kafka;
using Domain.Infrastructure;
using Microsoft.Extensions.Options;
using RentCommandApi.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Producers
{
    public class EventProducer : IEventProducer
    {
        private readonly ProducerConfig _config;
        private readonly ILoggerManager _logger;
        public EventProducer(
            IOptions<ProducerConfig> config,
            ILoggerManager logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        public async Task ProduceAsync<T>(
            string topic,
            T @event) where T : BaseEvent
        {
            using var producer = new ProducerBuilder<string, string>(_config)
               .SetKeySerializer(Serializers.Utf8)
               .SetValueSerializer(Serializers.Utf8)
               .Build();

            var eventMessage = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = JsonSerializer.Serialize(@event, @event.GetType())
            };

            var deliveryResult = await producer.ProduceAsync(topic, eventMessage);

            if (deliveryResult.Status == PersistenceStatus.NotPersisted)
            {
                throw new Exception($"Could not produce {@event.GetType().Name} message to topic - {topic} due to the following reason: {deliveryResult.Message}.");
            }

            _logger.LogInformation($"The {@event.GetType()} was produced to kafka");
        }
    }
}
