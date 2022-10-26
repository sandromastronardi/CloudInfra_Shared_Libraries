// Copyright (c) Mastronardi Software (mastrosoft.com). All Rights Reserved.
// Licensed under Mozilla Public License, Version 2.0

using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Eveneum;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace CompanyName.Shared.EventStore
{

    public class EventStoreFactory : IEventStoreFactory
    {
        private Lazy<Task<IEventStore>> _lazyEventStore;
        private const string _partitionKey = "/StreamId";

        public EventStoreFactory(IConfiguration configuration)
        {
            _lazyEventStore = new Lazy<Task<IEventStore>>(() => CreateEventStore(configuration));
        }

        public IEventStore GetEventStore()
        {
            return _lazyEventStore.Value.Result;
        }

        private static async Task<IEventStore> CreateEventStore(IConfiguration configuration)
        {
            var cosmos = configuration.GetSection("EventStore");
            var database = cosmos["Database"];
            var collection = cosmos["Collection"];
            var connectionString = cosmos["ConnectionString"];

            var client = new CosmosClient(connectionString, new CosmosClientOptions
            {
                SerializerOptions = new CosmosSerializationOptions
                {
                    IgnoreNullValues = true,
                    //PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            });
            var databaseResponse = await client.CreateDatabaseIfNotExistsAsync(database);
            await databaseResponse.Database.CreateContainerIfNotExistsAsync(new ContainerProperties(collection, _partitionKey));

            IEventStore eventStore = new Eveneum.EventStore(client, database, collection,new EventStoreOptions
            {
                JsonSerializer = JsonSerializer.Create(new JsonSerializerSettings {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                })
            });
            
            await eventStore.Initialize();

            return eventStore;
        }
    }
}