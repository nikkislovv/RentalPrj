using Domain.Infrastructure;
using MediatR;
using RentCommandApi.Commands;

namespace RentCommandApi.Handlers.CommandHendlers
{
    public class FinishRentHandler : IRequestHandler<FinishRentCommand>
    {
        private readonly IEventSourcingHandler _eventSourcingHandler;
        private readonly ILoggerManager _logger;

        public FinishRentHandler(
            IEventSourcingHandler eventSourcingHandler,
            ILoggerManager logger)
        {
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Unit> Handle(
            FinishRentCommand request,
            CancellationToken cancellationToken)
        {
            var rentAggregate = await _eventSourcingHandler.GetByIdAsync(request.Id, cancellationToken);
            rentAggregate.FinishRent();

            await _eventSourcingHandler.SaveAsync(rentAggregate, cancellationToken);

            _logger.LogInformation($"The finish event for rent with id:{request.Id} created");

            return Unit.Value;
        }
    }
    
}
