using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CompanyName.Shared.Events
{
    public class EventGridPublisherService : IEventGridPublisherService
    {
        private readonly Uri _topicEndpointUri;
        private readonly string _topicEndpointHostname;
        private readonly TopicCredentials _topicCredentials;
        private readonly string _defaultTopic;
        private readonly string _topicKey;
        private readonly string _domainEndpoint;
        private ISystemClock _systemClock;
        private readonly ILogger _logger;

        public EventGridPublisherService(
            ISystemClock systemClock,
            IOptions<EventGridSettings> settings,
            ILogger<EventGridPublisherService> logger = null
            )
        {
            _systemClock = systemClock;
            _logger = logger;
            _domainEndpoint = settings.Value.Endpoint?.ToString();
            _topicEndpointHostname = settings.Value.Endpoint?.Host;
            _defaultTopic = settings.Value.DefaultTopic;
            _topicKey = settings.Value.Key;
            _topicCredentials = new TopicCredentials(_topicKey);
        }
        public EventGridPublisherService()
        {
            _domainEndpoint = Environment.GetEnvironmentVariable("EventGridTopicEndpoint") ?? throw new Exception("EventGridTopicEndpoint Configuration missing");
            // get the connection details for the Event Grid topic
            _topicEndpointUri = new Uri(_domainEndpoint);
            _topicEndpointHostname = _topicEndpointUri.Host;
            _topicKey = Environment.GetEnvironmentVariable("EventGridTopicKey") ?? throw new Exception("EventGridTopicKey Configuration missing");
            _topicCredentials = new TopicCredentials(_topicKey);
            _defaultTopic = Environment.GetEnvironmentVariable("EventGridTopic");
        }

        public Task PostEventAsync<T>(string type, T payload, string topic = null) where T : EventSchemas.IEventMessage
        {
            if(payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            return PostEventAsync(type, payload.Subject, payload, topic);
        }
        [Obsolete("Use PostEventAsync instead")]
        public Task PostEventGridEventAsync<T>(string type, string subject, T payload, string topic = null)
        {
            return PostEventAsync<T>(type, subject, payload, topic);
        }
        public async Task PostEventAsync<T>(string type, string subject, T payload, string topic = null) {
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentNullException(nameof(subject));
            }
            if (!subject.Contains("/"))
            {
                throw new NotSupportedException("The subject must contain a / to separate identity and entity");
            }
            // prepare the events for submission to Event Grid
            var events = new List<Microsoft.Azure.EventGrid.Models.EventGridEvent>
            {
                new Microsoft.Azure.EventGrid.Models.EventGridEvent
                {
                    Id = Guid.NewGuid().ToString(),
                    EventType = type,
                    Topic = topic ?? _defaultTopic,
                    Subject = subject,
                    EventTime = _systemClock?.UtcNow.UtcDateTime ?? DateTime.UtcNow,
                    Data = payload,
                    DataVersion = payload?.GetType().Assembly.GetName().Version.ToString() ?? "0"
                }
            };

            // publish the events
            var client = new EventGridClient(_topicCredentials);
            var result = await client.PublishEventsWithHttpMessagesAsync(_topicEndpointHostname, events);
            if (!result.Response.IsSuccessStatusCode)
            {
                _logger?.LogError($"Error sending event: {type}", result.Response.ReasonPhrase, result.Response.Content);
            }
        }
}

    }