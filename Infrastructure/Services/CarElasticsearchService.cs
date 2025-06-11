using Domain.Entities;
using Domain.Infrastructure;
using Domain.Infrastructure.Services;
using Nest;

namespace Infrastructure.Services
{
    public class CarElasticsearchService : ICarElasticsearchService
    {
        private readonly IElasticClient _elasticClient;

        public CarElasticsearchService(
            IElasticClient elasticClient)
        {
            _elasticClient = elasticClient ?? throw new ArgumentNullException(nameof(elasticClient));
        }

        public async Task<Car> GetAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            var response = await _elasticClient.GetAsync<Car>(id, null, cancellationToken);

            return response.Source;
        }

    }
}
