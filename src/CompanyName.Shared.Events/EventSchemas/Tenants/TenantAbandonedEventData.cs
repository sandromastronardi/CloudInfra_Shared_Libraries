using System;

namespace CompanyName.Shared.Events.EventSchemas.Customers
{
    public class TenantAbandonedEventData : EventBase, IEventMessage
    {
        public TenantAbandonedEventData() { }
        public TenantAbandonedEventData(string id) : base(id)
        {
            Source = new Uri($"customers/{id}", UriKind.Relative);
        }
        public string Subject => Source.ToString();
        
        public string AbandonedBy { get; set; }
        public DateTime AbandonedOn { get; set; }
    }
}