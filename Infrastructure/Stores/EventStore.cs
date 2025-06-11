using Domain.Aggregates;
using Domain.Events;
using Domain.Exceptions;
using Domain.Infrastructure;
using Domain.Infrastructure.Repositories;
using Infrastructure.Producers;
using RentCommandApi.Events;


namespace Infrastructure.Stores
{
    public class EventStore : IEventStore
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerManager _logger;
        private readonly IEventProducer _eventProducer;
        public EventStore(
            IEventRepository eventRepository,
            ILoggerManager logger,
            IEventProducer eventProducer)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _eventProducer = eventProducer ?? throw new ArgumentNullException(nameof(eventProducer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<BaseEvent>> GetEventsAsync(
            Guid aggregateId,
            CancellationToken cancellationToken)
        {
            var events = await _eventRepository.FindByAggregateId(aggregateId, cancellationToken);

            if (events == null || !events.Any())
                throw new AggregateNotFoundException("Incorrect rent ID provided!");

            _logger.LogInformation(events.Count > 0
              ? $"retrived event(s) "
              : $"can't retrive events");

            return events.OrderBy(x => x.Version).Select(x => x.EventData).ToList();
        }

        public async Task SaveEventsAsync(
            Guid aggregateId,
            IEnumerable<BaseEvent> events,
            int expectedVersion,
            Guid carId,
            CancellationToken cancellationToken)
        {
            var existingEvents = await _eventRepository.FindByAggregateId(aggregateId, cancellationToken);

            if (expectedVersion != -1 && existingEvents[^1].Version != expectedVersion)
                throw new ConcurrencyException();

            var version = expectedVersion;

            foreach (var @event in events)
            {
                version++;
                @event.Version = version;
                var eventModel = new EventModel
                {
                    TimeStamp = DateTime.UtcNow,
                    AggregateIdentifier = aggregateId,
                    AggregateType = nameof(RentAggregate),
                    Version = version,
                    EventType = @event.GetType().Name,
                    CarId = carId,
                    EventData = @event
                };

                await _eventRepository.SaveAsync(eventModel, cancellationToken);

                var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
                await _eventProducer.ProduceAsync(topic, @event);
            }
        }

    }
}
