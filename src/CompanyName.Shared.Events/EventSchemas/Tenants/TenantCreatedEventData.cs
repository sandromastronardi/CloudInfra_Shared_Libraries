using System;

namespace CompanyName.Shared.Events.EventSchemas.Customers
{
    public class TenantCreatedEventData : CreatedEventBase
    {
        public TenantCreatedEventData() { }
        public TenantCreatedEventData(string id) : base(id)
        {
            Source = new Uri($"customers/{id}", UriKind.Relative);
        }

    }
}