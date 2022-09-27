using System;

namespace CompanyName.Shared.Events.EventSchemas
{
    public abstract class DisabledEventBase : EventBase , IEventMessage
    {


        public DisabledEventBase() { }
        public DisabledEventBase(string id) : base(id)
        {
        }
        public string Subject => Source.ToString();
        public string DisabledBy { get; set; }
        public DateTime DisabledOn { get; set; }
    }
}