namespace AStay.Common.Responses.Success
{
    public class PagedResponseEnvelope<T> : ResponseEnvelope<T[]> where T : class
    {
        public PagedResponse Pagination { get; set; }
    }
}