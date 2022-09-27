using System;

namespace CompanyName.Shared.Events.EventSchemas.Storage
{
    public class StorageFileCreatedEventData : CreatedEventBase
    {
        public StorageFileCreatedEventData() { }
        public StorageFileCreatedEventData(OwnerTypes ownerType, Guid ownerId, string file) : base(file)
        {
            OwnerType = ownerType;
            OwnerId = ownerId;
            Source = new Uri($"{ownerType.ToString().ToLowerInvariant()}s/{ownerId}/storage/files/{file?.TrimStart('/')}", UriKind.Relative);
        }

        public string Url { get; set; }
        public string Path { get; set; }
        public OwnerTypes OwnerType { get; set; }
        public Guid OwnerId { get; set; }
    }
}