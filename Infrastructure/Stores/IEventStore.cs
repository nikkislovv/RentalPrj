using RentCommandApi.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Infrastructure
{
    public interface IEventStore
    {
        Task SaveEventsAsync(
            Guid aggregateId,
            IEnumerable<BaseEvent> events,
            int expectedVersion,
            Guid carId,
            CancellationToken cancellationToken);

        Task<List<BaseEvent>> GetEventsAsync(
            Guid aggregateId,
            CancellationToken cancellationToken);
    }
}
