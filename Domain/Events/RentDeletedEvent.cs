namespace RentCommandApi.Events
{
    public class RentDeletedEvent : BaseEvent
    {
        public RentDeletedEvent() 
            : base(nameof(RentDeletedEvent))
        {
        }
    }
}
