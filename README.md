# Shared Libraries for microservices

When using my ARM templates for microservices I also use libraries for sending messages, using the event store, etc... these libraries are a basis for a microservice application framework.

This set of libraries can be used in your application environment setup with my ARM templates.
They assume eachothers conventions to work together.  
Up next are the specific libraries explained and how to use them.

## Clients
Each application you make will have other clients connecting to them.  In case of microservices it will be other services talking to your service.  
When these are both in C# it is usefull to provide the client to use to your peers.  They can install your client and easily connect to the endpoints you provide.
To facilitate the creation of these clients you can use this shared library as the underlying base for simplifying things like authentication.

The `BaseClient` contains all basic operations to execute on an API (GET,POST,DELETE,PUT).  The `ServiceBaseClient` class is the one to inherit in your client.
When you do that you can implement the `public Func<string> GetBearerToken` by assigning your implementation to your client.  This allows you to configure how this will be retrieved.

The `IdentitySettings` class can be added as an `IOptions` and will contain the required settings for connecting  to external services like your service.
If you dont configure a `BaseUrl` and an `Audience` in your appsettings it will create defaults based on your `AppEnvironment`.
This can be `Dev`, `Development`, `Test`, `Testing`, `Acc`, `Acceptance`, `Staging` and will autoconfigure these settings.
Do change those settings when you copy this code to reflect your own environment.  
If these values don't work for your specific sub-environment you can just override them as usual.

This is how you setup the configuration:

    builder.Services.AddOptions<IdentitySettings>()
    .Configure<IConfiguration>((settings, configuration) =>
    {
        config.GetSection(nameof(IdentitySettings)).Bind(settings);
    });

It will set the appsettings with naming like `IdentitySettings:Scope` (or `IdentitySettings__Scope`) in your object.

You can do the same with your own client settings that are required to connect to your service:

    builder.Services.AddOptions<MyServiceClientSettings>()
        .Configure<IConfiguration>((settings, configuration) =>
        {
            config.GetSection(nameof(MyServiceClientSettings)).Bind(settings);
        });

Your `MyServiceClientSettings` must inherit from `BaseSettings`, it will also have the autoconfiguration functionality for your app.

config comes from higher up in your `Configure` method in your `Startup.cs`

        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

To get your service to use the faster Utf8Json library for returning your json you can do this:

    builder.Services.AddMvcCore()
        .AddMvcOptions(options =>
        {
            // remove the existing JSON formatter
            //options.OutputFormatters.RemoveType(typeof(Microsoft.AspNetCore.Mvc.Formatters.JsonOutputFormatter));
            //options.InputFormatters.RemoveType(typeof(Microsoft.AspNetCore.Mvc.Formatters.JsonInputFormatter));

            options.OutputFormatters.RemoveType(typeof(Microsoft.AspNetCore.Mvc.Formatters.SystemTextJsonOutputFormatter));
            options.InputFormatters.RemoveType(typeof(Microsoft.AspNetCore.Mvc.Formatters.SystemTextJsonInputFormatter));

            options.OutputFormatters.Add(new Utf8Json.AspNetCoreMvcFormatter.JsonOutputFormatter(resolver));
            options.InputFormatters.Add(new Utf8Json.AspNetCoreMvcFormatter.JsonInputFormatter(resolver));
        });

(remove either SystemText json or the NewtonSoft json, depending on your application and what is loaded)

To use caching you can add the distributed cache:

            builder.Services.AddDistributedMemoryCache();

MemoryCache is for local development, you can add your own caching infrastructure the way you want.
The `IDistributedCache` is used in your `ServiceBaseClient` so you must add it to your environment.

# Authentication

to add the Authentication service you can do this in your `Startup.cs`

    builder.Services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();

In your function do:

    try{
        var userIdentity = await _userAuthenticationService.GetUserAsync(req);

        ...

    }catch (Exception ex) when (
            ex is AuthenticationExpectedException ||
            ex is SecurityTokenExpiredException
        )
    {
        return new UnauthorizedResult();
    }
    catch (Exception ex)
    {
        log.LogError(ex, "Error occured");
        return new BadRequestObjectResult(new InternalServerErrorErrorResponse(ex.Message));
    }

(`InternalServerErrorErrorResponse` can be found in `Mastronardi.Utils.Mvc` nuget)

# Common

The common libraries contain common classes to be used across your microservices.
 
## Api

These are classes for using in your API's.

`ListResponseBase` is an abstract baseclass for listing objects in your response. It helps to make your API restfull by providing paging information and related `_links`.
This is how you create your own List:

    public class CustomerResultList : ListResponseBase<CustomerResponse, ModelPagingLinks>
    {
        public CustomerResultList() { }
        public CustomerResultList(PagingParameters paging, long totalItems) : base(paging, totalItems) { }
    }

You can modify the response if you like, but it will inherit the default settings.

The ModelPagingLinks is a links response that contains all paging links to do paging.
The CustomerResponse is your item itself for returning an item to the requester:

    public class CustomerResponse : CustomerBase
    {
        public Guid Id { get; set; }
    }

This is the base class with all properties:

    public abstract class CustomerBase : ModelBase<CustomerLinks>
    {
        public string Owner { get; set; }
        [Required, MaxLength(50)]
        public string FirstName { get; set; }
        [Required, MaxLength(50)]
        public string LastName { get; set; }
        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; }
        [Required, MaxLength(30)]

        ...
        
        public bool? IsAbandoned { get; set; }
        public bool? IsDeleted { get; set; }

        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }

The Data Annotations are used for validation on your API endpoint.
As you can see your `CustomerBase` inherits from ModelBase with a type CustomerLinks (As your link base):

    public class CustomerLinks : ModelLinksBase
    {
        public CustomerLinks() { }
        public CustomerLinks(string root, Guid id)
        {
            Self = root + '/' + id;
            Users = Self + "/users";
        }

        public string Users { get; set; }
    }

When you use this on the backend you can return the links for self and for the list of users under this customer.
To use in your Mastronardi.Utils.Azure.Functions.FromQuery query in your function you can use the `PagingParameters`:

    public class PagingParameters
    {
        [Range(1, double.PositiveInfinity)]
        public int Page { get; set; } = 1;
        [Range(1,250)]
        public int PageSize { get; set; } = 25;
        public string NextPageToken { get; set; }
    }

With this object you can define the page you want to get the results for.  This object has Validation that you can use by using IValidated in your function.

Something similar exists for search:

    public class SearchParameters
    {
        public string Query { get; set; }
    }

And there is an `OrderByParameters` for ordering your results.

When you want to return a result you can do it like this in your API:

    var result = new CustomerResultList(pagingQuery.Value, totalItems)
    {
        Items = customers.Select(c =>
        {
            c.Links = new CustomerLinks(string.Empty, c, true);
            return c;
        }).ToList(),
        NextPageToken = nextPageToken,
        Links = new ModelPagingLinks($"/customers/{customerId}", pagingQuery.Value, totalItems, req.RequestUri.Query)
    };
    return new OkObjectResult(result);

You get all your customers and add a link object to it for building the Url's.
and you add a Links object to the root object for all the paging. here the default ModelPagingLinks is used, but you can ofcourse inherit this to add more properties.
Just make sure you use these too in your Clients.
`pagingQuery.Value` is your `PagingParameters` instance that is in your function signature with the FromQuery to pass it on to rebuild the paging Url's.

## Error Responses

Under Responses/Error you will find these responses to use:

  *  `BadRequestErrorResponse` (for bad requests, invalid content, query, etc.)
  *  `ErrorResponse` (base class)
  *  `ForbidErrorResponse` (for authentication errors)
  *  `InternalServerErrorErrorResponse` (for internal server errors, to display them in a recognizable format)
  *  `NotFoundErrorResponse` (to return when an item is not found, to be uses with status 404)
  *  `ValidationFailedErrorResponse` (the response to return when object validation failed)
     *  You can call this by using this extension method: `GetValidatorErrorResponse(this IEnumerable<ValidationResult> validationResults)` to convert a list of `ValidationResult`s to this response object.

    public enum NotificationLevel
    {
        Info,
        Warning,
        Error
    }

This `NotificationLevel` can be used to return a `Notification` of a specific type.

You can use the static methods under `Notification` to create a notification.

## Events

When using microservices applications must be notified across services when changes or actions occur.  The recommended high schale approach to this is using the Event Grid in Azure.
When using the event grid you can receive notifications as http calls on your endpoint, or get them as messages on a queue.
The event library is used to define all app-global events and the functionality to handle it.
All events are created here as they must be available everywhere in your application.  Keep in mind that they are events, not full data objects as they might be stale and the order is not guaranteed.
The best way to use this is to retrieve the relevant data in a WorkerApi when the event comes in.  When this retrieval fails, the handling of the event will fail and event grid will do a retry later (with a built in back off)

This is how you add the event grid functionality in your `Startup.cs`:

    // Events
    builder.Services.AddScoped<IEventGridPublisherService, CloudEventPublisherService>();
    builder.Services.AddScoped<ICloudEventsSubscriberService, CloudEventsSubscriberService>();
    builder.Services.AddOptions<EventGridSettings>()
        .Configure<IConfiguration>
    ((settings, configuration) =>
    {
    configuration.GetSection(nameof(EventGridSettings)).Bind(settings);
    });

    When you use the predefined ARM templates from me (https://github.com/sandromastronardi/ARM_Cloud_Infrastructure) the settings for Event Grid will already be added to your function app and they can be loaded with the code above.
    There are 2 services: The Subscriber and the Publisher.  The Subscriber service can be used to receive message,s the publisher will publish messages to the event grid.

    We have 2 types of event grid schemas. The models native to Azure, and the CloudEvents schema.  For future compatability with external services the CloudEvents schema is recommended.

    When you want to use the default these are the services to use
    *  `EventGridSubscriberService` and the `IEventGridSubscriberService`  interface
    *  `EventGridPublisherService`

    When you want to opt for the Cloud Events schema you use these services:
    *  `CloudEventsSubscriberService` and the `ICloudEventsSubscriberService` interface
    *  `CloudEventsPublisherService`

    To publish, for both schema's, you configure the `EventGridTopicEndpoint` appsetting.  This is done automatically when you use my ARM templates.
    The `EventGridTopicKey` is for authentication and `EventGridTopic` is the topic you want to publish your message to.
    To subscribe there are currently 2 interfaces, and also 2 services.

    To use the CloudEvents schema you need an `OPTIONS` handler like this:

    private readonly string[] eventEndpoints = new string[]{
    "downloader",
    "owners/users",
    "owners/partners",
    "owners/customers",
    "jobstorage"
    };

    [FunctionName(nameof(EventHandlersValidationTrigger))]
    public async Task<HttpResponseMessage>
        EventHandlersValidationTrigger(
        [HttpTrigger(AuthorizationLevel.Anonymous, "options", Route = "eventhandlers/{*path}")] HttpRequestMessage req,
        string path,
        ILogger log)
        {
        try
        {
        if (!eventEndpoints.Contains(path, StringComparer.OrdinalIgnoreCase))
        {
        var error = path + " is invalid for this handler";
        log.LogCritical(error);
        throw new InvalidOperationException(error);
        }
        var response = await _eventGridSubscriberService.HandleSubscriptionValidationEvent(req);
        return response;
        }
        catch (Exception ex)
        {
        log.LogError(ex.Demystify(), "Unhandled exception");
        return await Task.FromResult(req.CreateResponse(HttpStatusCode.InternalServerError, new InternalServerErrorErrorResponse(ex.Message)));
        }
        }

        This example allows multiple endpoints to exist for validation, you can ofcourse make one for each path you want, but like this everything after eventhandlers/* will be able to validate.

        A function like below can deconstruct the message for you:

        try
        {
        var (eventGridEvent, userId, _) = _eventGridSubscriberService.DeconstructEventGridMessage(req);

        if (eventGridEvent.Type == EventTypes.Customer.CustomerDeleted)
        {
        // cleanup of customer data
        }else if(eventGridEvent.Type == EventTypes.Partner.PartnerDeleted)
        {
        // cleanup of partner data
        }else if(eventGridEvent.Type == EventTypes.Users.UserCreated)
        {
        // cleanup of user data
        }
        _eventGridSubscriberService.OperationCompleted(true);
        return new OkResult();
        }
        catch (Exception ex)
        {
        log.LogError(ex.Demystify(), "Unhandled exception");
        _eventGridSubscriberService.OperationCompleted(false);
        return new InternalServerErrorObjectResult(new InternalServerErrorErrorResponse(ex.Message));
        }
        }

        the `OperationCompleted` method can be called to finalize the operation... This has nothing to do with finalizing the message itself, as this is done with a 200 status code, but it finalizes the operation for logging telemetry.
        This is usefull for tracking operations across services.

        ### How to define the type of events

        In the `EventTypes` class you can define all event types your application supports:

        public static class EventTypes
        {
        public const string SystemEventIdentity = "System";
        public static class Users
        {
        public const string UserCreated = nameof(UserCreated);
        public const string UserDeleted = nameof(UserDeleted);
        public const string UserUpdated = nameof(UserUpdated);
        public const string UserDisabled = nameof(UserDisabled);
        public const string UserEnabled = nameof(UserEnabled);
        public const string UserLoggedIn = nameof(UserLoggedIn);
        }
        }

        ...

        public static class Microsoft
        {
        public static class Storage
        {
        public const string BlobCreated = "Microsoft.Storage.BlobCreated";
        public const string BlobDeleted = "Microsoft.Storage.BlobDeleted";
        // more events https://docs.microsoft.com/en-us/azure/event-grid/event-schema-blob-storage
        }
        public static class KeyVault
        {
        public const string SecretNewVersionCreated  = "Microsoft.KeyVault.SecretNewVersionCreated";
        public const string SecretNearExpiry         = "Microsoft.KeyVault.SecretNearExpiry";
        public const string SecretExpired            = "Microsoft.KeyVault.SecretExpired";
        public const string VaultAccessPolicyChanged = "Microsoft.KeyVault.VaultAccessPolicyChanged";
        // more events https://docs.microsoft.com/en-us/azure/event-grid/event-schema-key-vault

        }
        public static class Devices
        {
        public const string DeviceCreated = "Microsoft.Devices.DeviceCreated";
        public const string DeviceDeleted = "Microsoft.Devices.DeviceDeleted";
        public const string DeviceConnected = "Microsoft.Devices.DeviceConnected";
        public const string DeviceDisconnected = "Microsoft.Devices.DeviceDisconnected";
        public const string DeviceTelemetry = "Microsoft.Devices.DeviceTelemetry";
        }
        }

        As you can see there are also the default event names of microsoft as you might want to receive these too.

        As you can see this is merely the naming of the events and it explains nothing on the data it contains.  These are defined under the folder `EventSchemas` and
        can be used for both schemas.

        This is what a UserCreated event looks like:

        public class UserCreatedEventData : CreatedEventBase
        {
        public UserCreatedEventData() { }
        public UserCreatedEventData(string id) : base(id)
        {
        Source = new Uri($"users/{id}", UriKind.Relative);
        }
        }

        it contains the path for the object with its id, and it will contain the ID in the parent class:

        public abstract class CreatedEventBase : EventBase , IEventMessage
        {
        public CreatedEventBase() { }
        public CreatedEventBase(string id) : base(id)
        {
        }
        public string Subject => Source.ToString();

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        }

        And the `EventBase` contains even more generic information for your events:

        public abstract class EventBase
        {
        protected string _subject;
        public EventBase() { }
        public EventBase(string id)
        {
        Guid guidId;
        if (id != null && Guid.TryParse(id, out guidId))
        {
        Id = guidId;
        }
        }
        public Guid Id { get; set; }
        public Uri Source { get; set; }
        public Guid UserId { get; set; }
        public string OperationId { get; set; }
        public string OperationParentId { get; set; }
        }

        it contains the `Id` here, as well as the `UserId` of the user doing the action and an `OperationId` and `OperationParentId` for tracking telemetry across services.
        This tracking is implemented in here by default for publishing as well as for subscribing, but you need to call the `OperationCompleted` method at the end with True for successfull completion, and false for unsuccessfull completion.

        You can create as many Events and EventSchemas as your application needs. This should be overseen by an event architect who makes sure no weed grows in between the events.

        ### How to send events after saving

        The best way to 'detect' a change is to use an event store, that way you can have your WorkerApi trigger on the event from the event store by listening to your Change Feed from CosmosDB and then send your events to the grid.

        private string GetEventName(string evt)
        {
        if (evt == null) return null;
        var fqdn = evt.Split(',')[0];
        var eventName = fqdn.Split('.').Last();
        return eventName?.Trim();
        }
        [FunctionName(nameof(CosmosUpdateEventTrigger))]
        public async Task CosmosUpdateEventTrigger([CosmosDBTrigger(
        databaseName:"%CosmosDBDatabase%",
        collectionName: "%CosmosDBEventCollection%",
        ConnectionStringSetting = "CosmosDB",
        LeaseCollectionName = "leases",
        LeaseCollectionPrefix = "partnerUpdateEvents",
        CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document>
            input,
            ILogger log)
            {
            if (input != null && input.Count > 0)
            {
            log.LogInformation("Documents modified " + input.Count);
            foreach (Document doc in input)
            {
            if (doc.GetPropertyValue<string>
                ("DocumentType") == "Event")
                {
                if (doc.Id.StartsWith("partner-"))
                {
                var stream = doc.GetPropertyValue<string>
                    ("StreamId");
                    var version = doc.GetPropertyValue<ulong>
                        ("Version");
                        var eventName = GetEventName(doc.GetPropertyValue<string>
                            ("BodyType"));
                            var partner = await _partnerEventRepository.GetPartner(stream, version);

                            switch (eventName)
                            {
                            case nameof(PartnerCreatedEvent):
                            await _eventSender.PostEventAsync(EventTypes.Partner.PartnerCreated, new PartnerCreatedEventData(new Guid(partner.Id))
                            {
                            CreatedOn = partner.CreatedOn,
                            CreatedBy = partner.CreatedBy.ToString(),
                            UserId = partner.CreatedBy
                            }); break;
                            case nameof(PartnerDeletedEvent):
                            await _eventSender.PostEventAsync(EventTypes.Partner.PartnerDeleted, new PartnerDeletedEventData(new Guid(partner.Id))
                            {
                            DeletedBy = partner.ModifiedBy.ToString(),
                            DeletedOn = partner.ModifiedOn,
                            UserId = partner.ModifiedBy
                            }); break;
                            case nameof(PartnerAbandonedEvent):
                            await _eventSender.PostEventAsync(EventTypes.Partner.PartnerAbandoned, new PartnerAbandonedEventData(new Guid(partner.Id))
                            {
                            AbandonedBy = partner.ModifiedBy.ToString(),
                            AbandonedOn = partner.ModifiedOn,
                            UserId = partner.ModifiedBy

                            }); break;
                            case nameof(PartnerDisabledEvent):
                            await _eventSender.PostEventAsync(EventTypes.Partner.PartnerDisabled, new PartnerDisabledEventData(new Guid(partner.Id))
                            {
                            DisabledBy = partner.ModifiedBy.ToString(),
                            DisabledOn = partner.ModifiedOn,
                            UserId = partner.ModifiedBy
                            }); break;
                            default:
                            log.LogWarning($"Event {eventName} not handled");break;
                            }
                            }
                            }
                            }
                            }
                            }

                            `_eventSender` is your publisher with which you send your events to the grid. As you can see all eents are detected and trigger the correct event to be sent.
                            It fills the information it needs and will send the event to the grid.  It uses the version because when 2 events are triggered after eachother, it makes sure it sends the current information relevant for this event.
                            Remember, your receiving end must not assume the last one is the most recent one, it must make sure it can receive the message multiple times too... (retries)
                            Best is to check the date modified on the receiving end to make sure it is not replayed...

                            ## Event Store

                            Eventstores are a way of storing data without ever removing old date and this way keeping history of all your changes.  Doing this with the Azure CosmosDB is easy peasy but you need a good library to do this.
                            This library uses `Eveneum` at the base and this implementation is the layer over it.

                            First there are 3 configuration values:

                            *  `EventStore:Database` (or: `EventStore__Database`)
                            *  `EventStore:Collection` (or: `EventStore__Collection`)
                            *  `EventStore:ConnectionString` (or: `EventStore__ConnectionString`)

                            In Database you set the name for the database, in Collection the collection inside that datbase and ConnectionString is the connection string for connecting to CosmosDB

                            You use the `EventStoreFactory`  (implements interface `IEventStoreFactory`) to get your event store:
                            Use this method: `GetEventStore()`

                            A better way to do this is to use the `EventStoreRepository` (implements interface `IEventStoreRepository`)
                            This wraps the EventStore and also adds telemetry tracking (for example when you want to keep track of the change in the Changefeed of CosmosDB)

                            You can:

                            `AddEventAsync(string streamId, object body, object metadata = null)`
                            If you add your own MetaData object it will not be able to track your changes, but if you inherit from MetaData this function will also add the operationId's

                            Other operations are
                            *  `public async Task<Response>
                                AddSnapshot(string streamId, ulong version, object snapshot)`
                                *  `public async Task<Response>
                                    DeleteSnapshots(string streamId, ulong version)`
                                    *  `public async Task<List<StreamHeader>> GetStreamHeaders()`
  *  `public async Task<Stream?> GetStream(string streamId)`
  *  `public async Task<DeleteResponse> DeleteStream(string streamId)`
  *  `public async Task<List<EventData>> GetEvents(string streamId)`
  *  `private async Task<ulong?> GetStreamVersionAsync(string streamId)`

A stream is a list of changes, but ofcourse when you want to browse trough the current version you might want to project this into one object or record.
This can be done by listening to the CosmosDB Changefeed and save it as one record (either in another CosmosDB Collection or a Sql Database) (see at Creating your repository)

### Creating events

Now how do you create an event?

    public abstract class PartnerEventBase : EventBase, IEvent<Documents.Partner>
    {
        public virtual void Apply(Partner target)
        {
            target.ModifiedBy = this.UserId;
            target.ModifiedOn = this.TimeStamp;
        }
    }

    public class PartnerCreatedEvent : PartnerEventBase
    {
        public bool Disabled { get; set; }
        public string CompanyName { get; set; }
        public string VatNumber { get; set; }
        public Address Address { get; set; }
        public Contact Contact { get; set; }
        public Contact TechContact { get; set; }
        public Contact BillingContact { get; set; }
        public Guid Owner { get; set; }
        public string BillingPO { get; set; }

        public override void Apply(Partner target)
        {
            target.Disabled = this.Disabled;
            target.CompanyName = this.CompanyName;
            target.VatNumber = this.VatNumber;
            target.Owner = this.Owner;
            target.BillingPO = this.BillingPO;

            if (this.Contact != null)
            {
                target.ContactEmail = this.Contact.Email;
                target.ContactFirstName = this.Contact.FirstName;
                target.ContactLastName = this.Contact.LastName;
                target.ContactPhone = this.Contact.LastName;
            }
            if (this.TechContact != null)
            {
                target.TechEmail = this.TechContact.Email;
                target.TechFirstName = this.TechContact.FirstName;
                target.TechLastName = this.TechContact.LastName;
                target.TechPhone = this.TechContact.LastName;
            }
            if (this.BillingContact != null)
            {
                target.BillingEmail = this.BillingContact.Email;
                //target.BillingFirstName = this.BillingContact.FirstName;
                //target.BillingLastName = this.BillingContact.LastName;
                //target.BillingPhone = this.BillingContact.LastName;
            }

            base.Apply(target);

            target.CreatedBy = target.ModifiedBy;
            target.CreatedOn = target.ModifiedOn;
        }
    }

You implement the `IEvent<>` interface that has a generic parameter which is the document to which you want to 'flatten' your event stream.
By implementing the `Apply` method you can define how this translates to your flat document that you will persist to another type.
In this example the `PartnerCreatedEvent` event inherits from a `PartnerEventBase` to apply he `ModifiedBy`and `ModifiedOn` properties.

### Creating your repository

Here you see an example for an Event Repository for partners.
It wraps your IEventStoreRepository to do operations on your stream.  The `GetPartner` method gets your your partner document by running your stream and applying all operations.
When you specify a version it stops at that version.

    public class PartnerEventRepository : IPartnerEventRepository
    {
        private readonly IEventStoreRepository _eventStoreRepository;

        public PartnerEventRepository(IEventStoreRepository eventStoreRepository)
        {
            _eventStoreRepository = eventStoreRepository;
        }
        private string GetPartnerStreamId(Guid partnerId)
        {
            return $"partner-{partnerId}";
        }
        public async Task<Guid> AddPartner(PartnerCreatedEvent partnerAddedEvent)
        {
            var partnerId = Guid.NewGuid();
            await _eventStoreRepository.AddEventAsync(GetPartnerStreamId(partnerId), partnerAddedEvent);
            return partnerId;
        }
        public async Task<Guid> AddPartner(Guid partnerId, PartnerCreatedEvent partnerCreatedEvent)
        {
            await _eventStoreRepository.AddEventAsync(GetPartnerStreamId(partnerId), partnerCreatedEvent);
            return partnerId;
        }

        public async Task ContractSigned(Guid partnerId)
        {
            await _eventStoreRepository.AddEventAsync(GetPartnerStreamId(partnerId), new ContractSignedEvent());
        }

        public async Task Delete(Guid partnerId)
        {
            await _eventStoreRepository.AddEventAsync(GetPartnerStreamId(partnerId), new PartnerDeletedEvent());
        }

        public async Task ValidateAddress(Guid partnerId, string code)
        {
            await _eventStoreRepository.AddEventAsync(GetPartnerStreamId(partnerId), new AddressValidatedEvent(code));
        }
        public Task<Partner> GetPartner(Guid partnerId)
        {
            return GetPartner(GetPartnerStreamId(partnerId));
        }
        public async Task<Partner> GetPartner(string partnerStreamId, ulong? version = null)
        {
            var (stream, id) = await GetStream(partnerStreamId, version);
            if (!stream.HasValue)
            {
                return null;
            }
            var p = new Partner() { Id = id.ToString() };

            foreach (var evt in stream.Value.Events)
            {
                if (!version.HasValue || evt.Version <= version)
                {

                    var body = evt.Body as IEvent<Documents.Partner>;
                    if (body != null)
                    {
                        body.Apply(p);
                    }
                }
            }
            return p;
        }

        public async Task<(Eveneum.Stream?,Guid)> GetStream(string partnerStreamId, ulong? version = null)
        {
            _ = partnerStreamId ?? throw new ArgumentNullException(nameof(partnerStreamId));
            if (!partnerStreamId.StartsWith("partner-"))
            {
                throw new NotSupportedException("Invalid partner stream Id");
            }
            var streamParts = partnerStreamId.Split('-', 2);
            Guid id;
            if (!Guid.TryParse(streamParts[1], out id))
            {
                throw new NotSupportedException("Invalid partner stream Id, not a guid");
            }
            var stream = await _eventStoreRepository.GetStream(partnerStreamId);
            if (!stream.HasValue)
            {
                return (null,Guid.Empty);
            }
            return (stream, id);
        }
    }

This way you have your repository for working with your EventStore and replaying all information to your document or entity for sql.
The best way to persist your events to a document or sql is to work with the CosmosDB Change Feed like this:

    [FunctionName(nameof(CosmosUpdateTrigger))]
    [Disable("HandlersDisabled")]
    public async Task CosmosUpdateTrigger([CosmosDBTrigger(
        databaseName:"%CosmosDBDatabase%",
        collectionName: "%CosmosDBEventCollection%",
        ConnectionStringSetting = "CosmosDB",
        LeaseCollectionName = "leases",
        LeaseCollectionPrefix = "partnerUpdates",
        CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input,
        ILogger log)
    {
        if (input != null && input.Count > 0)
        {
            log.LogInformation("Documents modified " + input.Count);
            foreach (Document doc in input)
            {
                if (doc.Id.StartsWith("partner-"))
                {
                    var stream = doc.GetPropertyValue<string>("StreamId");
                    var partner = await _partnerEventRepository.GetPartner(stream);
                    await _container.UpsertItemAsync(partner);
                }


            }
        }
    }

Every change triggers this function and it will persist each object to the latest version.
It retrieves the stream by getting the `StreamId`, retrieving the partner from the `_partnerEventRepository` and then does an upsert to the `_container`_ which
in this case is another collection where all flat objects are stored in CosmosDB.

## Resilience (Polly)

The Reslilience library helps to create standardised policies for circuit breakers in your services.
This is currently not complete.

# How to start

You start by doing a search and replace for CompanyName which you replace with your company name...
this must be done for namespaces, but you must also rename the projects and the folders.

## Compiling & release

These libraries will be used across your application and must be compiled.
You can use the provided pipeline yaml's and build them, and then after that you can deploy the artifacts to your internal nuget feed.
You can then use these libraries in all your applications and you have a central codebase for usign these functionalities.
When a developer needs a new event he can have it checked by the responsable for the events and when its approved he can use them in his application.  Everyone that wants to subscribe to these events can then also subscribe to them.
Best is to keep a document describing each event for each service so subscribers know what they can subscribe to.
