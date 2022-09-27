using AStay.Common.Pagination;
using Microsoft.AspNetCore.Http;

namespace AStay.Common.Responses.Success
{
    public class PagedResponse : PaginationData
    {
        public PaginationLinks Links { get; }

        public PagedResponse(PaginationData paginationData)
        {
            Page = paginationData.Page;
            PageSize = paginationData.PageSize;
            TotalItems = paginationData.TotalItems;
        }
    }
}