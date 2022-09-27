#if (NETCOREAPP)
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using CompanyName.Shared.Authentication;

using CompanyName.Shared.Events;
using CompanyName.Shared.Events.CloudEvents;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;

namespace CompanyName.Shared.Tests
{
    [TestClass]
    public class TestEvents
    {
        string eventJson = @"[
  {
    ""topic"": ""/subscriptions/{subscription-id}/resourceGroups/Storage/providers/Microsoft.Storage/storageAccounts/xstoretestaccount"",
    ""subject"": ""identity/831e1650-001e-001b-66ab-eeb76e069631"",
    ""eventType"": ""PrintJobCreated"",
    ""eventTime"": ""2017-06-26T18:41:00.9584103Z"",
    ""id"": ""831e1650-001e-001b-66ab-eeb76e069631"",
    ""data"": {
      ""subject"":""831e1650-001e-001b-66ab-eeb76e069631"",
      ""createdBy"":""test"",
      ""createdOn"":""2017-06-26T18:41:00.9584103Z""
    },
    ""dataVersion"": """",
    ""metadataVersion"": ""1""
  }
]";

        [TestMethod]
        public void TestCanDeserialize()
        {
            /*HttpRequestMessage dummyRequest = new HttpRequestMessage(HttpMethod.Post, "http://localhost:/download")
            {
                Content = new StreamContent(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(eventJson)))
            };*/
            IEventGridSubscriberService subscriber = new EventGridSubscriberService();
            var context = new DefaultHttpContext();
            context.Request.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(eventJson));
            var (evt, user, item) = subscriber.DeconstructEventGridMessage(context.Request);

        }

        [TestMethod]
        public void TestCloudEventsSubscriptionHandler()
        {
            var settings = Options.Create<EventGridSettings>(new EventGridSettings
            {
                DefaultTopic = "test",
                WebHookAllowedOrigin = "eventgrid.azure.net",
                WebHookAllowedRate = "*"
            });
            ICloudEventsSubscriberService subscriber = new CloudEventsSubscriberService(new SystemClock(),settings,null);
            var req = new HttpRequestMessage { };
            req.Method = HttpMethod.Options;
            req.Headers.Add("Webhook-Request-Origin", "eventgrid.azure.net");
            req.Headers.Add("Webhook-Request-Rate", "*");
            req.Headers.Add("Webhook-Request-Callback", "https://www.google.com");
            var response = subscriber.HandleSubscriptionValidationEvent(req).GetAwaiter().GetResult();
            Assert.AreEqual(response.Content.Headers.GetValues("Allow").FirstOrDefault(), "POST");
            Assert.AreEqual(response.Headers.GetValues("Webhook-Allowed-Origin").FirstOrDefault(), "eventgrid.azure.net");
            Assert.IsNotNull(response.Headers.GetValues("Webhook-Allowed-Rate").FirstOrDefault());
        }

    }
}
#endif