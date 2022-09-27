using System;

namespace CompanyName.Shared.Events.EventSchemas.Devices
{
    public class DeviceUpdatedEventData : UpdatedEventBase
    {
        public Guid TenantId { get; set; }
        public DeviceUpdatedEventData() { }
        public DeviceUpdatedEventData(Guid tenantId, Guid id) : base(id.ToString())
        {
            TenantId = tenantId;
            Source = new Uri($"tenants/{tenantId}/devices/{id}", UriKind.Relative);
        }
    }
}