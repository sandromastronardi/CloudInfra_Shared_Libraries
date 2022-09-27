using CloudNative.CloudEvents;
using System.Net.Http;
using System.Threading.Tasks;

namespace CompanyName.Shared.Events.CloudEvents
{
    public interface ICloudEventsSubscriberService
    {
        (CloudEvent cloudEvent, string userId, string itemId) DeconstructEventGridMessage(HttpRequestMessage req);
        Task<HttpResponseMessage> HandleSubscriptionValidationEvent(HttpRequestMessage req);
        void OperationCompleted(bool success);
    }
}