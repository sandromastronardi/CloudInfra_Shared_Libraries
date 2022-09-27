using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CloudNative.CloudEvents.Extensions;
using CloudNative.CloudEvents;
using System.IO;
using System.Net.Mime;
using CompanyName.Shared.Events.EventSchemas;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace CompanyName.Shared.Events.CloudEvents
{
    public class CloudEventsSubscriberService : ICloudEventsSubscriberService, IDisposable
    {
        private readonly ISystemClock _systemClock;
        private readonly ILogger<EventGridPublisherService> _logger;
        private readonly EventGridSettings _settings;
        private readonly TelemetryClient _telemetryClient;
        private IOperationHolder<RequestTelemetry> _operation;

        public class CustomCloudEventFormatter : ICloudEventFormatter
        {
            private readonly JsonEventFormatter _innerFormatter;

            public CustomCloudEventFormatter()
            {
                _innerFormatter = new JsonEventFormatter();
            }

            public object DecodeAttribute(CloudEventsSpecVersion specVersion, string name, byte[] data, IEnumerable<ICloudEventExtension> extensions)
            {
                return ((ICloudEventFormatter)_innerFormatter).DecodeAttribute(specVersion, name, data, extensions);
            }

            public CloudEvent DecodeStructuredEvent(Stream data, IEnumerable<ICloudEventExtension> extensions)
            {
                return ((ICloudEventFormatter)_innerFormatter).DecodeStructuredEvent(data, extensions);
            }

            public CloudEvent DecodeStructuredEvent(byte[] data, IEnumerable<ICloudEventExtension> extensions)
            {
                return ((ICloudEventFormatter)_innerFormatter).DecodeStructuredEvent(data, extensions);
            }

            public Task<CloudEvent> DecodeStructuredEventAsync(Stream data, IEnumerable<ICloudEventExtension> extensions)
            {
                return ((ICloudEventFormatter)_innerFormatter).DecodeStructuredEventAsync(data, extensions);
            }

            public byte[] EncodeAttribute(CloudEventsSpecVersion specVersion, string name, object value, IEnumerable<ICloudEventExtension> extensions)
            {
                return ((ICloudEventFormatter)_innerFormatter).EncodeAttribute(specVersion, name, value, extensions);
            }

            public byte[] EncodeStructuredEvent(CloudEvent cloudEvent, out ContentType contentType)
            {
                return ((ICloudEventFormatter)_innerFormatter).EncodeStructuredEvent(cloudEvent, out contentType);
            }
        }
        public CloudEventsSubscriberService(
            ISystemClock systemClock,
            IOptions<EventGridSettings> settings,
            TelemetryClient telemetryClient,
            ILogger<EventGridPublisherService> logger = null)
        {
            _systemClock = systemClock;
            _logger = logger;
            _settings = settings.Value ?? CloudEventPublisherService.GetDefaultSettings();
            _telemetryClient = telemetryClient;
        }
        public CloudEventsSubscriberService()
        {
            _settings = CloudEventPublisherService.GetDefaultSettings();
        }

        public (CloudEvent cloudEvent, string userId, string itemId) DeconstructEventGridMessage(HttpRequestMessage req)
        {

            var cloudEventResponse = req.ToCloudEvent();

            cloudEventResponse.Data = EventGridSubscriberService.CreateStronglyTypedDataObject(cloudEventResponse.Data, cloudEventResponse.Type);
            string userId = null;
            string itemId = null;
            IEventMessage evtMsg = null;
            if (cloudEventResponse.Data is IEventMessage)
            {
                //string[] subjectParts = cloudEventResponse.Subject.Split('/', 2);
                //if (subjectParts.Length > 1)
                //{
                //    Guid dataId;
                //    if (Guid.TryParse(subjectParts[1], out dataId))
                //    {
                //        ((IEventMessage)cloudEventResponse.Data).Id = dataId;
                //    }
                //}
                evtMsg = ((IEventMessage)cloudEventResponse.Data);
                itemId = evtMsg.Id.ToString();
                userId = evtMsg.UserId.ToString();
            }
            if (evtMsg != null && !string.IsNullOrWhiteSpace(evtMsg.OperationId))
            {
                _operation = _telemetryClient?.StartOperation<RequestTelemetry>(
                   "Handle " + cloudEventResponse.Type,
                   evtMsg.OperationId,
                   evtMsg.OperationParentId);
            }
            // find the user ID and item ID from the subject
            //var eventGridEventSubjectComponents = cloudEventResponse.Subject.Split('/');
            //if (eventGridEventSubjectComponents.Length != 2)
            //{
            //    throw new InvalidOperationException("Event Grid event subject is not in expected format.");
            //}
            //var userId = eventGridEventSubjectComponents[0];
            //var itemId = eventGridEventSubjectComponents[1];

            return (cloudEventResponse, userId, itemId);
        }

        public async Task<HttpResponseMessage> HandleSubscriptionValidationEvent(HttpRequestMessage req)
        {
            if (!req.IsWebHookValidationRequest())
            {
                throw new NotSupportedException("The request is not a WebHook validation request!");
            }
            if (!req.Headers.Contains("WebHook-Request-Rate"))
            {
                req.Headers.Add("WebHook-Request-Rate", "*");
            }
            if (req.Headers.Contains("Webhook-Request-Callback"))
            {
                req.Headers.Remove("Webhook-Request-Callback");
            }

            var response = await req.HandleAsWebHookValidationRequest((origin) =>
            {
                var setting = _settings.WebHookAllowedOrigin ?? Environment.GetEnvironmentVariable("WebHook-Request-Origin");
                if (string.IsNullOrEmpty(setting) || setting == "*")
                {
                    return true;
                }
                else
                {
                    return setting == origin;
                }
            }, (o) =>
            {
                return _settings.WebHookAllowedRate ?? Environment.GetEnvironmentVariable("WebHook-Request-Rate") ?? "*";
            });
            return response;
        }

        public void OperationCompleted(bool success)
        {
            if (_operation != null)
            {
                if (success)
                {
                    _operation.Telemetry.Success = true;
                    _operation.Telemetry.ResponseCode = "200";
                }
                else
                {
                    _operation.Telemetry.Success = false;
                    _operation.Telemetry.ResponseCode = "500";
                }
                _telemetryClient.StopOperation(_operation);
                _operation = null;
            }
        }
        public void Dispose()
        {
            if(_operation != null)
            {
                _telemetryClient.StopOperation(_operation);
            }
        }
    }
}
