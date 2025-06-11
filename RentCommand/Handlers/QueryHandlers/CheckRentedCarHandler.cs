using Domain.Infrastructure;
using Domain.Infrastructure.Repositories;
using MediatR;
using RentCommandApi.Events;
using RentCommandApi.Queries;

namespace RentCommandApi.Handlers.QueryHandlers
{
    public class CheckRentedCarHandler : IRequestHandler<CheckRentedCarQuery, bool>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventSourcingHandler _eventSourcingHandler;

        public CheckRentedCarHandler(
            IEventRepository eventRepository,
            IEventSourcingHandler eventSourcingHandler)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
        }

        public async Task<bool> Handle(
            CheckRentedCarQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateIds = (await _eventRepository.FindByCarId(request.Id, cancellationToken))
                .GroupBy(x => x.AggregateIdentifier).Select(x => x.Key);

            foreach (var id in aggregateIds)
            {
                var rentAggregate = await _eventSourcingHandler.GetByIdAsync(id, cancellationToken);

                if (rentAggregate.Status == "Finished")
                    continue;

                if (request.Start != DateTime.MinValue && (request.Start >= rentAggregate.Start && request.Start <= rentAggregate.Finish))
                    return true;
                else if (request.Finish >= rentAggregate.Start && request.Finish <= rentAggregate.Finish)
                    return true;
            }

            return false;
        }
    }
}
