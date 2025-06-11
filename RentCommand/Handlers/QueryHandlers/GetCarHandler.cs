using Domain.Entities;
using Domain.Exceptions;
using Domain.Infrastructure;
using Domain.Infrastructure.Services;
using MediatR;
using RentCommandApi.Queries;

namespace RentCommandApi.QueryHandlers
{
    public class GetCarHandler : IRequestHandler<GetCarQuery, Car>
    {
        private readonly ICarElasticsearchService _carElasticsearchService;
        private readonly ILoggerManager _logger;

        public GetCarHandler(
            ICarElasticsearchService carElasticsearchService,
            ILoggerManager logger)
        {
            _carElasticsearchService = carElasticsearchService ?? throw new ArgumentNullException(nameof(carElasticsearchService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Car> Handle(
            GetCarQuery request,
            CancellationToken cancellationToken)
        {
            var car = await _carElasticsearchService.GetAsync(request.Id, cancellationToken);

            _logger.LogInformation(car != null
              ? $"retrived a car by id: {request.Id}"
              : $"can't retrive a car by id: {request.Id}");

            return car;
        }
    }
}
