using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.Shared.Events
{
    public class EventGridSettings
    {
        public EventGridSettings()
        {
            //var domainEndpoint = Environment.GetEnvironmentVariable("EventGridTopicEndpoint");
            //// get the connection details for the Event Grid topic
            //Endpoint = !string.IsNullOrWhiteSpace(domainEndpoint) ? new Uri(domainEndpoint) : null;
            //Key = Environment.GetEnvironmentVariable("EventGridTopicKey");
            //WebHookAllowedOrigin = Environment.GetEnvironmentVariable("WebHook-Allowed-Origin");
            //WebHookAllowedRate = Environment.GetEnvironmentVariable("WebHook-Allowed-Rate");
        }

        public Uri Endpoint { get; set; }
        public string DefaultTopic { get; set; }
        public string Key { get; set; }
        public string WebHookAllowedOrigin { get; set; }
        public string WebHookAllowedRate { get; set; }
    }
}
