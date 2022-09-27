using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.Shared.EventStore
{
    public class MetaData
    {
        public string OperationId { get; set; }
        public string OperationParentId { get; set; }
    }
}
