namespace CompanyName.Shared.Events.EventSchemas.Microsoft.Storage
{
    public abstract class BlobBaseEventData
    {
        public string Api { get; set; }
        public string RequestId { get; set; }
        public string ETag { get; set; }
        public string ContentType { get; set; }
        public long ContentLength { get; set; }
        public string BlobType { get; set; }
        public string Url { get; set; }
        public string Sequencer { get; set; }
        public Storagediagnostics StorageDiagnostics { get; set; }
    }

}
