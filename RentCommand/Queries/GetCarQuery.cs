using Domain.Entities;
using Domain.Queries;
using MediatR;

namespace RentCommandApi.Queries
{
    public class GetCarQuery : BaseQuery, IRequest<Car>
    {
    }
}
