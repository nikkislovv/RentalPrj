namespace RentCommandApi.Events
{
    public class RentFinishedEvent : BaseEvent
    {
        public RentFinishedEvent() 
            : base(nameof(RentFinishedEvent))
        {
        }
        public string Status { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
