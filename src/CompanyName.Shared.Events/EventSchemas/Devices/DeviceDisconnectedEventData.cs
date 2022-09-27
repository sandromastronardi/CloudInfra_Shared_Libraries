using System;

namespace CompanyName.Shared.Events.EventSchemas.Devices
{
    public class DeviceDisconnectedEventData : EventBase , IEventMessage
    {
        public Guid PartnerId { get; set; }
        public DeviceDisconnectedEventData() { }
        public DeviceDisconnectedEventData(Guid tenantId, Guid id) : base(id.ToString())
        {
            PartnerId = tenantId;
            Source = new Uri($"tenants/{tenantId}/devices/{id}", UriKind.Relative);
        }

        public string Subject => Source.ToString();
    }
}