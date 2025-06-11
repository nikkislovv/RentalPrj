using Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Infrastructure
{
    public interface IEventSourcingHandler
    {
        Task SaveAsync(
            RentAggregate aggregate,
            CancellationToken cancellationToken);
        Task<RentAggregate> GetByIdAsync(
            Guid aggregateId,
            CancellationToken cancellationToken);
    }
}
