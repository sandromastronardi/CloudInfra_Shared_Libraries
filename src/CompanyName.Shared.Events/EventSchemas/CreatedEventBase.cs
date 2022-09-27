using System;

namespace CompanyName.Shared.Events.EventSchemas
{
    public abstract class CreatedEventBase : EventBase , IEventMessage
    {
        public CreatedEventBase() { }
        public CreatedEventBase(string id) : base(id)
        {
        }
        public string Subject => Source.ToString();

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}