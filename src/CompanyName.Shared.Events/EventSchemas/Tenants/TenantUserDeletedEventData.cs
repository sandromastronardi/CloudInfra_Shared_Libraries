using System;

namespace CompanyName.Shared.Events.EventSchemas.Customers
{
    public class TenantUserDeletedEventData : DeletedEventBase
    {
        public TenantUserDeletedEventData() { }
        public TenantUserDeletedEventData(string id) : base(id)
        {
            Source = new Uri($"customers/{id}", UriKind.Relative);
        }
        public string DeletedUser { get; set; }
    }
}