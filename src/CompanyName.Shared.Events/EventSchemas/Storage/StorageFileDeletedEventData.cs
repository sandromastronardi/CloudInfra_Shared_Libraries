using System;

namespace CompanyName.Shared.Events.EventSchemas.Storage
{
    public class StorageFileDeletedEventData : DeletedEventBase
    {
        public StorageFileDeletedEventData() { }
        public StorageFileDeletedEventData(OwnerTypes ownerType, Guid ownerId, string file) : base(file)
        {
            Source = new Uri($"{ownerType.ToString().ToLowerInvariant()}s/{ownerId}/storage/files/{file?.TrimStart('/')}", UriKind.Relative);
        }
        public string Url { get; set; }
        public string Path { get; set; }
    }
}