using System;

namespace CompanyName.Shared.Events.EventSchemas.Devices
{
    public class DeviceCommandFailedEventData : EventBase, IEventMessage
    {
        public Guid TenantId { get; set; }
        public DeviceCommandFailedEventData() { }
        public DeviceCommandFailedEventData(Guid partnerId, Guid id) : base(id.ToString())
        {
            TenantId = partnerId;
            Source = new Uri($"tenants/{partnerId}/devices/{id}", UriKind.Relative);
        }

        public string Subject => Source.ToString();

        public string ExecutedBy { get; set; } = EventTypes.SystemEventIdentity;
        public DateTime ExecutedOn { get; set; }
    }
}