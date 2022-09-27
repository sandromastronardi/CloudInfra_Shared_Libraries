using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.Shared.Events.EventSchemas.Microsoft.KeyVault
{
    public class Secret
    {
        public string Id { get; set; }
        public string VaultName { get; set; }
        public string ObjectType { get; set; }
        public string ObjectName { get; set; }
        public string Version { get; set; }
        public long Nbf { get; set; }
        public long Exp { get; set; }
    }

}
