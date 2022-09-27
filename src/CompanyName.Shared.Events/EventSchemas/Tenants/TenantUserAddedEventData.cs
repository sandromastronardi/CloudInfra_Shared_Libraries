using System;

namespace CompanyName.Shared.Events.EventSchemas.Customers
{
    public class TenantUserAddedEventData : EventBase, IEventMessage
    {
        public TenantUserAddedEventData() { }
        public TenantUserAddedEventData(string id) : base(id)
        {
            Source = new Uri($"customers/{id}", UriKind.Relative);
        }

        public string Subject => Source.ToString();

        public string AddedBy { get; set; }
        public DateTime AddedOn { get; set; }
        public string AddedUser { get; set; }
    }
}