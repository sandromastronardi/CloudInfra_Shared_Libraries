using System;

namespace CompanyName.Shared.Events.EventSchemas
{
    public abstract class UpdatedEventBase : EventBase , IEventMessage
    {
        public UpdatedEventBase() { }
        public UpdatedEventBase(string id) : base(id)
        {
        }
        public string Subject => Source.ToString();
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}