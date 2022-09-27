using System;

namespace CompanyName.Shared.Events.EventSchemas.Customers
{
    public class TenantDisabledEventData : DisabledEventBase
    {
        public TenantDisabledEventData()
        {
        }
        public TenantDisabledEventData(string id) : base(id)
        {
            Source = new Uri($"customers/{id}", UriKind.Relative);
        }
    }
}