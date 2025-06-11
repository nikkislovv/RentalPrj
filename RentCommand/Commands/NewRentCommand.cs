using MediatR;

namespace RentCommandApi.Commands
{
    public class NewRentCommand : BaseCommand, IRequest
    {
        public string UserId { get; set; }
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }
        public Guid CarId { get; set; }
        public decimal DayCost { get; set; }
    }
}
