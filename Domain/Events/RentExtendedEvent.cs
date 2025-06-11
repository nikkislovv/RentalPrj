namespace RentCommandApi.Events
{
    public class RentExtendedEvent : BaseEvent
    {
        public RentExtendedEvent() 
            : base(nameof(RentExtendedEvent))
        {
        }
        public DateTime Finish { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
