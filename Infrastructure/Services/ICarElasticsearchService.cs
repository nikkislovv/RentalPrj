using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Infrastructure.Services
{
    public interface ICarElasticsearchService
    {
        Task<Car> GetAsync(
            Guid id,
            CancellationToken cancellationToken);
    }
}
