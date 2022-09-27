using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CompanyName.Shared.Events.EventSchemas;
using CompanyName.Shared.Events.EventSchemas.Customers;
using CompanyName.Shared.Events.EventSchemas.Devices;
using CompanyName.Shared.Events.EventSchemas.Microsoft.Storage;
using CompanyName.Shared.Events.EventSchemas.Storage;
using CompanyName.Shared.Events.EventSchemas.Users;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CompanyName.Shared.Events
{
    public class EventGridSubscriberService : IEventGridSubscriberService
    {
        internal const string EventGridSubscriptionValidationHeaderKey = "Aeg-Event-Type";

        public async Task<IActionResult> HandleSubscriptionValidationEvent(HttpRequest req)
        {
            if (req.Body.CanSeek)
            {
                req.Body.Position = 0;
            }

            var requestBody = await (new StreamReader(req.Body).ReadToEndAsync());
            if (string.IsNullOrEmpty(requestBody))
            {
                return null;
            }

            dynamic data = JsonConvert.DeserializeObject(requestBody);

            foreach (var dataEvent in data)
            {
                if (req.Headers.TryGetValue(EventGridSubscriptionValidationHeaderKey, out StringValues values) && values.Equals("SubscriptionValidation") &&
                    dataEvent.eventType == "Microsoft.EventGrid.SubscriptionValidationEvent")
                {
                    // this is a special event type that needs an echo response for Event Grid to work
                    var validationCode = dataEvent.data.validationCode; // TODO .ToString();
                    var echoResponse = new EventValidationResponse { ValidationResponse = validationCode };
                    return new OkObjectResult(echoResponse);
                }
            }

            return null;
        }

        public async Task<IActionResult> HandleSubscriptionValidationEvent(HttpRequestMessage req)
        {
            var stream = await req.Content.ReadAsStreamAsync();
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            var requestBody = await (new StreamReader(stream).ReadToEndAsync());
            if (string.IsNullOrEmpty(requestBody))
            {
                return null;
            }

            dynamic data = JsonConvert.DeserializeObject(requestBody);

            foreach (var dataEvent in data)
            {
                if (req.Headers.Contains(EventGridSubscriptionValidationHeaderKey))
                {
                    var values = req.Headers.GetValues(EventGridSubscriptionValidationHeaderKey);
                    if (values.Contains("SubscriptionValidation") &&
                    dataEvent.eventType == "Microsoft.EventGrid.SubscriptionValidationEvent")
                    {
                        // this is a special event type that needs an echo response for Event Grid to work
                        var validationCode = dataEvent.data.validationCode; // TODO .ToString();
                        var echoResponse = new EventValidationResponse{ ValidationResponse = validationCode };
                        return new OkObjectResult(echoResponse);
                    }
                }
            }

            return null;
        }

        public (EventGridEvent eventGridEvent, string userId, string itemId) DeconstructEventGridMessage(HttpRequest req)
        {
            // read the request stream
            if (req.Body.CanSeek)
            {
                req.Body.Position = 0;
            }
            var requestBody = new StreamReader(req.Body).ReadToEnd();

            // deserialise into a single Event Grid event - we won't allow multiple events to be processed
            var eventGridEvents = JsonConvert.DeserializeObject<EventGridEvent[]>(requestBody);
            if (eventGridEvents.Length == 0)
            {
                return (null, null, null);
            }
            if (eventGridEvents.Length > 1)
            {
                throw new InvalidOperationException("Expected only a single Event Grid event.");
            }
            var eventGridEvent = eventGridEvents.Single();

            // convert the 'data' property to a strongly typed object rather than a JObject
            eventGridEvent.Data = CreateStronglyTypedDataObject(eventGridEvent.Data, eventGridEvent.EventType);
            if(eventGridEvent.Data is IEventMessage)
            {
                string[] subjectParts = eventGridEvent.Subject.Split('/',2);
                if (subjectParts.Length > 1)
                {
                    Guid dataId = Guid.Empty;
                    if (Guid.TryParse(subjectParts[1], out dataId))
                    {
                        ((IEventMessage)eventGridEvent.Data).Id = dataId;
                    }
                }
            }
            // find the user ID and item ID from the subject
            var eventGridEventSubjectComponents = eventGridEvent.Subject.Split('/');
            if (eventGridEventSubjectComponents.Length != 2)
            {
                throw new InvalidOperationException("Event Grid event subject is not in expected format.");
            }
            var userId = eventGridEventSubjectComponents[0];
            var itemId = eventGridEventSubjectComponents[1];

            return (eventGridEvent, userId, itemId);
        }

        internal static object CreateStronglyTypedDataObject(object data, string eventType)
        {
            switch (eventType)
            {
                // creates

                case EventTypes.Users.UserCreated:
                    return ConvertDataObjectToType<StorageJobFileCreatedEventData>(data);

                case EventTypes.Tenants.TenantCreated:
                    return ConvertDataObjectToType<TenantCreatedEventData>(data);

                case EventTypes.Device.DeviceCreated:
                    return ConvertDataObjectToType<DeviceCreatedEventData>(data);

                case EventTypes.Storage.StorageFileCreated:
                    return ConvertDataObjectToType<StorageFileCreatedEventData>(data);

                case EventTypes.Storage.StorageJobFileCreated:
                    return ConvertDataObjectToType<StorageJobFileCreatedEventData>(data);
                // updates

                case EventTypes.Tenants.TenantUpdated:
                    return ConvertDataObjectToType<TenantUpdatedEventData>(data);

                case EventTypes.Device.DeviceUpdated:
                    return ConvertDataObjectToType<DeviceUpdatedEventData>(data);

                case EventTypes.Storage.StorageFileUpdated:
                    return ConvertDataObjectToType<StorageFileUpdatedEventData>(data);

                case EventTypes.Storage.StorageJobFileUpdated:
                    return ConvertDataObjectToType<StorageJobFileUpdatedEventData>(data);

                // deletes

                case EventTypes.Tenants.TenantDeleted:
                    return ConvertDataObjectToType<TenantDeletedEventData>(data);

                case EventTypes.Device.DeviceDeleted:
                    return ConvertDataObjectToType<DeviceDeletedEventData>(data);

                case EventTypes.Storage.StorageFileDeleted:
                    return ConvertDataObjectToType<StorageFileDeletedEventData>(data);

                case EventTypes.Storage.StorageJobFileDeleted:
                    return ConvertDataObjectToType<StorageJobFileDeletedEventData>(data);

                //devices
                case EventTypes.Device.DeviceConnected:
                    return ConvertDataObjectToType<DeviceConnectedEventData>(data);
                case EventTypes.Device.DeviceDisconnected:
                    return ConvertDataObjectToType<DeviceDisconnectedEventData>(data);
                case EventTypes.Device.DeviceCommandFailed:
                    return ConvertDataObjectToType<DeviceCommandFailedEventData>(data);
                // Users other
                case EventTypes.Users.UserLoggedIn:
                    return ConvertDataObjectToType<UserLoggedInEventData>(data);

                // Azure Storage
                case EventTypes.Microsoft.Storage.BlobCreated:
                    return ConvertDataObjectToType<BlobCreatedEventData>(data);
                case EventTypes.Microsoft.Storage.BlobDeleted:
                    return ConvertDataObjectToType<BlobDeletedEventData>(data);

                // Azure IOT Hub
                case EventTypes.Microsoft.Devices.DeviceConnected:
                case EventTypes.Microsoft.Devices.DeviceDisconnected:
                case EventTypes.Microsoft.Devices.DeviceCreated:
                case EventTypes.Microsoft.Devices.DeviceDeleted:
                    return ConvertDataObjectToType<EventSchemas.Microsoft.Devices.DeviceData>(data);

                // Azure KeyVault
                case EventTypes.Microsoft.KeyVault.SecretNewVersionCreated:
                case EventTypes.Microsoft.KeyVault.SecretNearExpiry:
                case EventTypes.Microsoft.KeyVault.SecretExpired:
                    return ConvertDataObjectToType<EventSchemas.Microsoft.KeyVault.Secret>(data);
                default:
                    throw new ArgumentException($"Unexpected event type '{eventType}' in {nameof(CreateStronglyTypedDataObject)}");
            }
        }

        private static T ConvertDataObjectToType<T>(object dataObject)
        {
            if (dataObject is JObject o)
            {
                return o.ToObject<T>();
            }

            return (T)dataObject;
        }
    }
}