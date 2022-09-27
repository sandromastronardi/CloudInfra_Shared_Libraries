using System;

namespace CompanyName.Shared.Events.EventSchemas.Customers
{
    public class TenantVerifiedEventData : EventBase, IEventMessage
    {
        public TenantVerifiedEventData() { }
        public TenantVerifiedEventData(string id) : base(id)
        {
            Source = new Uri($"customers/{id}", UriKind.Relative);
        }
        public string Subject => Source.ToString();

        public string VerifiedBy { get; set; }
        public DateTime DateVerified { get; set; }
    }
}