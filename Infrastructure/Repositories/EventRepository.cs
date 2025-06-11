using Domain.Events;
using Domain.Infrastructure.Repositories;
using Infrastructure.Config;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly IMongoCollection<EventModel> _eventStoreCollection;

        public EventRepository(IOptions<MongoDbConfig> config)
        {
            var mongoClient = new MongoClient(config.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(config.Value.Database);

            _eventStoreCollection = mongoDatabase.GetCollection<EventModel>(config.Value.Collection);
        }

        public async Task<List<EventModel>> FindByAggregateId(
            Guid aggregateId,
            CancellationToken cancellationToken = default)
        {
            return await _eventStoreCollection.Find(x => x.AggregateIdentifier == aggregateId)
                .ToListAsync(cancellationToken);
        }

        public async Task SaveAsync(
            EventModel @event,
            CancellationToken cancellationToken = default)
        {
            await _eventStoreCollection.InsertOneAsync(@event, cancellationToken);
        }

        public async Task<List<EventModel>> FindByCarId(
            Guid carId,
            CancellationToken cancellationToken = default)
        {
            return await _eventStoreCollection.Find(x => x.CarId == carId)
                          .ToListAsync(cancellationToken);
        }
    }
}
