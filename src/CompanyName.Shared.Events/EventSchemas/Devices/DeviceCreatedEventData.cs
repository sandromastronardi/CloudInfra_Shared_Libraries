using System;

namespace CompanyName.Shared.Events.EventSchemas.Devices
{
    public class DeviceCreatedEventData : CreatedEventBase
    {
        public Guid TenantId { get; set; }
        public DeviceCreatedEventData() { }
        
        public DeviceCreatedEventData(Guid tenantId, string id) : base(id)
        {
            TenantId = tenantId;
            Source = new Uri($"tenants/{tenantId}/devices/{id}", UriKind.Relative);
        }
    }
}