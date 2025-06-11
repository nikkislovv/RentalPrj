using Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Infrastructure.Repositories
{
    public interface IEventRepository
    {
        Task SaveAsync(
            EventModel @event,
            CancellationToken cancellationToken = default);

        Task<List<EventModel>> FindByAggregateId(
            Guid aggregateId,
            CancellationToken cancellationToken = default);

        Task<List<EventModel>> FindByCarId(
            Guid carId,
            CancellationToken cancellationToken = default);
    }
}
