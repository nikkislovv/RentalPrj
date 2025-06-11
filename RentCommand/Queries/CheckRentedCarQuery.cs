using Domain.Queries;
using MediatR;

namespace RentCommandApi.Queries
{
    public class CheckRentedCarQuery : BaseQuery, IRequest<bool>
    {
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }
    }
}
