using System;

namespace CompanyName.Shared.Events.EventSchemas.Customers
{
    public class TenantUpdatedEventData : UpdatedEventBase
    {
        public TenantUpdatedEventData() { }
        public TenantUpdatedEventData(string id) : base(id)
        {
            Source = new Uri($"customers/{id}", UriKind.Relative);
        }

        //public string ModifiedBy { get; set; }
        //public DateTime DateModified { get; set; }
    }
}