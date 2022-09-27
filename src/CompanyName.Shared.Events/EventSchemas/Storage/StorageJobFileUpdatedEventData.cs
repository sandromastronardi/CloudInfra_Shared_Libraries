using System;

namespace CompanyName.Shared.Events.EventSchemas.Storage
{
    public enum JobFileType
    {
        Data, // The printable PDF.
        ConvertedData,
        Colors, // The colors.json file for the job data file.
        //ShippingLabel, // A shipping label
        TaskList // The worksheet explaining the work to be done on the job (added to the end of the job)
    }
    public class StorageJobFileUpdatedEventData : StorageFileUpdatedEventData
    {
        public StorageJobFileUpdatedEventData() { }
        public StorageJobFileUpdatedEventData(OwnerTypes ownerType, Guid ownerId, string file) : base(ownerType, ownerId, file)
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