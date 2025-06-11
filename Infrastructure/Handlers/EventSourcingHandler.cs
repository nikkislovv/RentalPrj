using Domain.Aggregates;
using Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Handlers
{
    public class EventSourcingHandler : IEventSourcingHandler
    {
        public readonly IEventStore _eventStore;

        public EventSourcingHandler(
            IEventStore eventStore)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
        }

        public async Task<RentAggregate> GetByIdAsync(
            Guid aggregateId,
            CancellationToken cancellationToken)
        {
            var aggregate = new RentAggregate();
            var events = await _eventStore.GetEventsAsync(aggregateId, cancellationToken);

            if (events == null || !events.Any()) return aggregate;

            aggregate.ReplayEvents(events);
            aggregate.Version = events.Select(x => x.Version).Max();

            return aggregate;
        }

        public async Task SaveAsync(
            RentAggregate aggregate,
            CancellationToken cancellationToken)
        {
            await _eventStore.SaveEventsAsync(aggregate.Id, aggregate.GetUncommittedChanges(), aggregate.Version, aggregate.CarId, cancellationToken);
            aggregate.MarkChangesAsCommitted();
        }
    }
}
