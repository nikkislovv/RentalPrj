namespace RentCommandApi.Events
{
    public abstract class BaseEvent
    {
        public Guid Id { get; set; }
        protected BaseEvent(string type)
        {
            Type = type;
        }

        public string Type { get; set; }
        public int Version { get; set; }
    }
}
