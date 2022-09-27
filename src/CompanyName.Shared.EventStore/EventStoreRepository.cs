using Eveneum;
using Eveneum.Advanced;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyName.Shared.EventStore
{

    public class EventStoreRepository : IEventStoreRepository
    {
        private readonly IEventStore _eventStore;
        private readonly TelemetryClient _telemetryClient;

        public EventStoreRepository(IEventStoreFactory eventStoreFactory, TelemetryClient telemetryClient)
        {
            _eventStore = eventStoreFactory.GetEventStore();
            _telemetryClient = telemetryClient;
        }

        public async Task<Response> AddEventAsync(string streamId, object body, object metadata = null)
        {
            var operationName = $"Add {body.GetType().Name} event";
            var operation = _telemetryClient?.StartOperation<DependencyTelemetry>(operationName);
            try
            {
                if (operation != null)
                {
                    operation.Telemetry.Type = "EventStore";
                    operation.Telemetry.Data = operationName;
                    if (metadata == null)
                    {
                        metadata = new MetaData
                        {
                            OperationId = operation.Telemetry.Context.Operation.Id,
                            OperationParentId = operation.Telemetry.Id
                        };
                    }
                    else if (metadata is MetaData)
                    {
                        var md = (MetaData)metadata;
                        if (string.IsNullOrEmpty(md.OperationId))
                        {
                            md.OperationId = operation.Telemetry.Context.Operation.Id;
                        }
                        if (string.IsNullOrEmpty(md.OperationParentId))
                        {
                            md.OperationParentId = operation.Telemetry.Id;
                        }
                    }
                }
                var expectedVersion = await GetStreamVersionAsync(streamId);
                var events = CreateEvents(streamId, body, metadata, expectedVersion);

                var result = await _eventStore.WriteToStream(streamId, events, expectedVersion);
                if(operation != null)
                {
                    operation.Telemetry.Success = true;
                }
                return result;
            }catch(Exception ex)
            {
                _telemetryClient?.TrackException(ex);
                if (operation != null)
                {
                    operation.Telemetry.Success = false;
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

        public async Task<Response> AddSnapshot(string streamId, ulong version, object snapshot)
        {
            return await _eventStore.CreateSnapshot(streamId, version, snapshot);
        }

        public async Task<Response> DeleteSnapshots(string streamId, ulong version)
        {
            return await _eventStore.DeleteSnapshots(streamId, version);
        }

        public async Task<List<StreamHeader>> GetStreamHeaders()
        {
            var headers = new List<StreamHeader>();
            var query = "SELECT * from c";

            await ((IAdvancedEventStore)_eventStore).LoadStreamHeaders(query, e =>
            {
                headers.AddRange(e);
                return Task.CompletedTask;
            });

            return headers;
        }

        public async Task<Stream?> GetStream(string streamId)
        {
            var streams = await _eventStore.ReadStream(streamId);
            return streams.Stream;
        }

        public async Task<DeleteResponse> DeleteStream(string streamId)
        {
            var expectedVersion = await GetStreamVersionAsync(streamId);
            return await _eventStore.DeleteStream(streamId, expectedVersion.Value);
        }

        public async Task<List<EventData>> GetEvents(string streamId)
        {
            List<EventData> events = new List<EventData>();

            var query = $"SELECT * FROM c WHERE c.StreamId = '{streamId}'";
            await ((IAdvancedEventStore)_eventStore).LoadEvents(query, e =>
            {
                events.AddRange(e);
                return Task.CompletedTask;
            });

            return events;
        }

        private EventData[] CreateEvents(string streamId, Object body, Object metadata, ulong? existingVersion)
        {
            var version = existingVersion.GetValueOrDefault() + 1;
            var data = new EventData(streamId, body, metadata, version,null);
            return new List<EventData> { data }.ToArray();
        }

        private async Task<ulong?> GetStreamVersionAsync(string streamId)
        {
            ulong? version = null;
            var query = $"SELECT TOP 1 * from c WHERE c.StreamId = '{streamId}'";

            await ((IAdvancedEventStore)_eventStore).LoadStreamHeaders(query, e =>
            {
                if (e.Count > 0)
                    version = e.First().Version;
                return Task.CompletedTask;
            });

            return version;
        }
    }
}