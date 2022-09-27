using System;

namespace CompanyName.Shared.Events.EventSchemas.Devices
{
    public class DeviceConnectedEventData : EventBase, IEventMessage
    {
        public Guid TenantId { get; set; }
        public DeviceConnectedEventData() { }
        public DeviceConnectedEventData(Guid tenantId, Guid id) : base(id.ToString())
        {
            TenantId = tenantId;
            Source = new Uri($"tenants/{tenantId}/devices/{id}", UriKind.Relative);
        }
        public string Subject => Source.ToString();
    }
}