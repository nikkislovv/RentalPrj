using Domain.Aggregates;
using Domain.Infrastructure;
using MediatR;
using RentCommandApi.Commands;

namespace RentCommandApi.CommandHendlers
{
    public class NewRentHandler : IRequestHandler<NewRentCommand>
    {
        private readonly IEventSourcingHandler _eventSourcingHandler;
        private readonly ILoggerManager _logger;

        public NewRentHandler(
            IEventSourcingHandler eventSourcingHandler,
            ILoggerManager logger)
        {
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Unit> Handle(
            NewRentCommand request,
            CancellationToken cancellationToken)
        {
            var rentAggregate = new RentAggregate(request.Id, request.UserId, request.Start, request.Finish, request.CarId, request.DayCost);

            await _eventSourcingHandler.SaveAsync(rentAggregate, cancellationToken);

            _logger.LogInformation($"The NewRent event for rent with id:{request.Id} created");

            return Unit.Value;
        }
    }
}
