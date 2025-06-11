using Domain.Infrastructure;
using MediatR;
using RentCommandApi.Commands;

namespace RentCommandApi.Handlers.CommandHendlers
{
    public class ExtendRentHandler : IRequestHandler<ExtendRentCommand>
    {
        private readonly IEventSourcingHandler _eventSourcingHandler;
        private readonly ILoggerManager _logger;
        public ExtendRentHandler(
            IEventSourcingHandler eventSourcingHandler,
            ILoggerManager logger)
        {
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<Unit> Handle(
            ExtendRentCommand request,
            CancellationToken cancellationToken)
        {
            var rentAggregate = await _eventSourcingHandler.GetByIdAsync(request.Id, cancellationToken);
            rentAggregate.ExtendRent(request.Finish, request.DayCost);

            await _eventSourcingHandler.SaveAsync(rentAggregate, cancellationToken);

            _logger.LogInformation($"The extend event for rent with id:{request.Id} created");

            return Unit.Value;
        }
    }
}
