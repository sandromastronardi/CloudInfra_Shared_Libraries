using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.Shared.Events.EventSchemas.Microsoft.Storage
{

    public class BlobCreatedEventData : BlobBaseEventData
    {
        public string ClientRequestId { get; set; }
    }

}
