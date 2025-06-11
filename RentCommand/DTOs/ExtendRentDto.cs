namespace RentCommandApi.DTOs
{
    public class ExtendRentDto
    {
        public DateTime Finish { get; set; }
        public Guid CarId { get; set; }
        public decimal DayCost { get; set; }
    }
}
