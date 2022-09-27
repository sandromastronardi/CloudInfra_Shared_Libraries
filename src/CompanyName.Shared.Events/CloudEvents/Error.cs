namespace CompanyName.Shared.Events.CloudEvents
{
    public class Error
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public Detail[] Details { get; set; }
    }

}
