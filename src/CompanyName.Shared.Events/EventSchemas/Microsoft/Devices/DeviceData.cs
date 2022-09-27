using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.Shared.Events.EventSchemas.Microsoft.Devices
{

    public class DeviceData
    {
        public DeviceConnectionStateEventInfo DeviceConnectionStateEventInfo { get; set; }
        public string HubName { get; set; }
        public string DeviceId { get; set; }
    }

    public class DeviceConnectionStateEventInfo
    {
        public string SequenceNumber { get; set; }
    }

}
