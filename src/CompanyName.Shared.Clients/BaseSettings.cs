using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyName.Shared.Clients
{
    public abstract class BaseSettings
    {
        public BaseSettings()
        {
            switch (Environment.GetEnvironmentVariable("AppEnvironment")?.ToLowerInvariant() ?? string.Empty)
            {
                case "dev":
                case "development":
                    BaseUrl = "https://api.companyname-dev.net";
                    break;
                case "test":
                case "testing":
                    BaseUrl = "https://api.companyname-test.net";
                    break;
                case "staging":
                case "acc":
                case "acceptance":
                    BaseUrl = "https://api.companyname-staging.net";
                    break;
                default:
                    BaseUrl = "https://api.companyname.com";
                    break;

            }
        }

        public string BaseUrl { get; set;}
        public string ApiKey { get; set; }
    }
}
