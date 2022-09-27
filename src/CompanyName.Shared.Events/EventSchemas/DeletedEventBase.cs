using System;

namespace CompanyName.Shared.Events.EventSchemas
{
    public abstract class DeletedEventBase : EventBase , IEventMessage
    {
        public DeletedEventBase() { }
        public DeletedEventBase(string id) : base(id)
        {
        }
        public string Subject => Source.ToString();
        public string DeletedBy { get; set; }
        public DateTime DeletedOn { get; set; }
    }
}