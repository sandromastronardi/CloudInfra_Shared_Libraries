namespace AStay.Common.Responses.Success
{
    public class ResponseEnvelope<T> where T : class
    {
        public T Data { get; set; }
    }
}