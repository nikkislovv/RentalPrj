using MediatR;

namespace RentCommandApi.Commands
{
    public class ExtendRentCommand : BaseCommand, IRequest
    {
        public DateTime Finish { get; set; }
        public Guid CarId { get; set; }
        public decimal DayCost { get; set; }
    }
}
