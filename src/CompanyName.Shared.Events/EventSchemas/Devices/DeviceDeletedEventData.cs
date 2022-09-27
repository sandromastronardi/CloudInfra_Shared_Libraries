using System;

namespace CompanyName.Shared.Events.EventSchemas.Devices
{
    public class DeviceDeletedEventData : DeletedEventBase
    {
        public Guid TenantId { get; set; }
        public DeviceDeletedEventData() { }
        public DeviceDeletedEventData(Guid tenantId, Guid id) : base(id.ToString())
        {
            TenantId = tenantId;
            Source = new Uri($"tenants/{tenantId}/devices/{id}", UriKind.Relative);
        }
    }
}