using System;

namespace CompanyName.Shared.Events.EventSchemas.Customers
{
    public class TenantDeletedEventData : DeletedEventBase
    {
        public TenantDeletedEventData() { }
        public TenantDeletedEventData(string id) : base(id)
        {
            Source = new Uri($"customers/{id}", UriKind.Relative);
        }

        //public string DeletedBy { get; set; }
        //public DateTime DateDeleted { get; set; }
    }
}