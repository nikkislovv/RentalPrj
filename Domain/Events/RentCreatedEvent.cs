namespace RentCommandApi.Events
{
    public class RentCreatedEvent : BaseEvent
    {
        public RentCreatedEvent()
            : base(nameof(RentCreatedEvent))
        {
        }
        public string UserId { get; set; }
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }
        public Guid CarId { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
