using System;

namespace CompanyName.Shared.Events.EventSchemas
{
    public abstract class EnabledEventBase : EventBase, IEventMessage
    {

        public EnabledEventBase() { }
        public EnabledEventBase(string id) : base(id)
        {
        }
        public string Subject => Source.ToString();
        public string EnabledBy { get; set; }
        public DateTime EnabledOn { get; set; }
    }
}