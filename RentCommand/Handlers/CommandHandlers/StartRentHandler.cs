using Domain.Infrastructure;
using MediatR;
using RentCommandApi.Commands;

namespace RentCommandApi.Handlers.CommandHendlers
{
    public class StartRentHandler : IRequestHandler<StartRentCommand>
    {
        private readonly IEventSourcingHandler _eventSourcingHandler;
        private readonly ILoggerManager _logger;

        public StartRentHandler(
            IEventSourcingHandler eventSourcingHandler,
            ILoggerManager logger)
        {
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Unit> Handle(
            StartRentCommand request,
            CancellationToken cancellationToken)
        {
            var rentAggregate = await _eventSourcingHandler.GetByIdAsync(request.Id, cancellationToken);
            rentAggregate.StartRent();

            await _eventSourcingHandler.SaveAsync(rentAggregate, cancellationToken);

            _logger.LogInformation($"The start event for rent with id:{request.Id} created");

            return Unit.Value;
        }
    }
}
