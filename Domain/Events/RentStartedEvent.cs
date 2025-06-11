namespace RentCommandApi.Events
{
    public class RentStartedEvent : BaseEvent
    {
        public RentStartedEvent() 
            : base(nameof(RentStartedEvent))
        {
        }
        public string Status { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
