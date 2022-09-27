using System;

namespace CompanyName.Shared.Events.EventSchemas.Storage
{
    public class StorageJobFileDeletedEventData : StorageFileDeletedEventData
    {
        public StorageJobFileDeletedEventData() { }
        public StorageJobFileDeletedEventData(OwnerTypes ownerType, Guid ownerId, string file) : base(ownerType, ownerId, file)
        {
            var (jobId, filePath) = FileNameParser.GetMetaData(file);
            Path = file;
            JobId = jobId;
            File = filePath;
            Source = new Uri($"{ownerType.ToString().ToLowerInvariant()}s/{ownerId}/storage/printjobs/{file?.TrimStart('/')}", UriKind.Relative);
        }

        public Guid JobId { get; set; }
        public string File { get; set; }
    }
}