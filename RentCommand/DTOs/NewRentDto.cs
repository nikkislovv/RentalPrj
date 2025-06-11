namespace RentCommandApi.DTOs
{
    public class NewRentDto
    {
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }
        public Guid CarId { get; set; }
    }
}
