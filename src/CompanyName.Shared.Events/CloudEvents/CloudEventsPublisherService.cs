using Azure;
using Azure.Messaging.EventGrid;
using CloudNative.CloudEvents;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using CompanyName.Shared.Events.EventSchemas;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace CompanyName.Shared.Events.CloudEvents
{
    public class CloudEventsPublisherService : IEventGridPublisherService
    {

        private readonly HttpClient _httpClient;
        private readonly ISystemClock _systemClock;
        private readonly ILogger<EventGridPublisherService> _logger;
        private readonly EventGridSettings _settings;
        private readonly TelemetryClient _telemetryClient;

        //private readonly Uri _topicEndpointUri;

        public CloudEventsPublisherService(
            HttpClient httpClient,
            ISystemClock systemClock,
            IOptions<EventGridSettings> settings,
            TelemetryClient telemetryClient,
            ILogger<EventGridPublisherService> logger = null)
        {
            _httpClient = httpClient;
            _systemClock = systemClock;
            _logger = logger;
            _settings = settings.Value ?? GetDefaultSettings();
            _telemetryClient = telemetryClient;
        }
        public CloudEventsPublisherService()
        {
            _settings = GetDefaultSettings();
        }

        internal static EventGridSettings GetDefaultSettings()
        {
            return new EventGridSettings
            {
                Endpoint = new Uri(Environment.GetEnvironmentVariable("EventGridTopicEndpoint") ?? throw new Exception("EventGridTopicEndpoint Configuration missing")),
                Key = Environment.GetEnvironmentVariable("EventGridTopicKey") ?? throw new Exception("EventGridTopicKey Configuration missing"),
                DefaultTopic = Environment.GetEnvironmentVariable("EventGridTopic"),
                WebHookAllowedOrigin = "eventgrid.azure.net",
                WebHookAllowedRate = "*"
            };
        }
        public Task PostEventAsync<T>(string type, T payload, string topic = null) where T : IEventMessage
        {
            return PostEventAsync(type, payload.Subject, payload, topic);
        }

        public async Task PostEventAsync<T>(string type, string subject, T payload, string topic = null)
        {
            Uri source = new Uri(topic ?? _settings.DefaultTopic, UriKind.Relative);


            //var cloudEvent = new CloudEvent(type, source)
            //{
            //    DataContentType = new ContentType(MediaTypeNames.Application.Json),
            //    Subject = subject,
            //    Data = payload,
            //    Id = Guid.NewGuid().ToString(),
            //    Time = _systemClock?.UtcNow.UtcDateTime ?? DateTime.UtcNow,
            //    SpecVersion = CloudEventsSpecVersion.V1_0
            //};

            //var content = new CloudEventContent(cloudEvent,
            //                                     ContentMode.Structured,
            //                                     new JsonEventFormatter());
            // https://github.com/Azure/azure-sdk-for-net/blob/master/sdk/eventgrid/Azure.Messaging.EventGrid/samples/Sample1_PublishEventsToTopic.md
            string operationName = "Publish " + type;
            var operation = _telemetryClient?.StartOperation<DependencyTelemetry>(operationName);
            if (operation != null)
            {
                operation.Telemetry.Type = "EventGrid";
                operation.Telemetry.Data = operationName;
            }
            try
            {
                var client = new EventGridPublisherClient(
                    _settings.Endpoint, new AzureKeyCredential(_settings.Key));


                if (payload is IEventMessage)
                {
                    var evtMsg = ((IEventMessage)payload);
                    subject = evtMsg.Source.ToString();
                    evtMsg.OperationId = operation.Telemetry.Context.Operation.Id;
                    evtMsg.OperationParentId = operation.Telemetry.Id;
                }

                var cloudEvent = new Azure.Messaging.CloudEvent(source.ToString(), type, payload)
                {
                    DataContentType = MediaTypeNames.Application.Json,
                    Time = _systemClock?.UtcNow.UtcDateTime ?? DateTime.UtcNow,
                    Subject = subject,
                    Id = Guid.NewGuid().ToString()
                };

                var result = await client.SendEventAsync(cloudEvent);
                //content.Headers.Add("aeg-sas-key", _settings.Key);
                //var result = await _httpClient.PostAsync(_settings.Endpoint, content);
                //if (!result.IsSuccessStatusCode)
                if (result.Status >= 200 && result.Status <= 299)
                {
                    if (operation != null)
                    {
                        operation.Telemetry.Success = true;
                    }
                }
                else
                {
                    if (operation != null)
                    {
                        operation.Telemetry.Success = false;
                    }
                    //var error = JsonConvert.DeserializeObject<ErrorResponse>(result.Content.ReadAsStringAsync(), new JsonSerializerSettings
                    //{
                    //    ContractResolver = 
                    //});
                    _logger?.LogError($"Error sending CloudEvent: {type}", result.ReasonPhrase, UTF8Encoding.UTF8.GetString(result.Content));//?.ReadAsStringAsync()) ;
                }
            }
            catch (Exception ex)
            {
                if (operation != null)
                {
                    operation.Telemetry.Success = false;
                    _telemetryClient?.TrackException(ex);
                }
                throw;
            }
            finally
            {
                if (operation != null)
                {
                    _telemetryClient?.StopOperation(operation);
                }
            }
        }
        [Obsolete("Use PostEventAsync instead")]
        public Task PostEventGridEventAsync<T>(string type, string subject, T payload, string topic = null)
        {
            return PostEventAsync(type, subject, payload, topic);
        }
    }
    [Obsolete("use CloudEventsPublisherService instead")]
    public class CloudEventPublisherService : CloudEventsPublisherService
    {
        public CloudEventPublisherService(
        HttpClient httpClient,
        ISystemClock systemClock,
        IOptions<EventGridSettings> settings,
        TelemetryClient telemetryClient,
        ILogger<EventGridPublisherService> logger = null):base(httpClient, systemClock, settings, telemetryClient, logger)
            {

        }
        public CloudEventPublisherService():base()
        {
        }
    }
}
