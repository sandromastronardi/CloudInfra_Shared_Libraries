using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyName.Shared.Common.Api
{

    public class ModelPagingLinks : ModelLinksBase
    {
        private const string QS_PAGESIZE = "pageSize";
        private const string QS_PAGE = "page";

        public ModelPagingLinks() { }
        public ModelPagingLinks(string root, PagingParameters pagingParameters, long totalItems, string queryString = null)
        {
            // https://stackoverflow.com/questions/29992848/parse-and-modify-a-query-string-in-net-core
            var query = QueryHelpers.ParseNullableQuery(queryString);
            if (query != null)
            {
                // Convert the StringValues into a list of KeyValue Pairs to make it easier to manipulate
                var items = query.SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value)).ToList();
                // At this point you can remove items if you want
                items.RemoveAll(x =>
                    x.Key.Equals("rnd", StringComparison.OrdinalIgnoreCase) ||
                    x.Key.Equals("r", StringComparison.OrdinalIgnoreCase) ||
                    x.Key.Equals("random", StringComparison.OrdinalIgnoreCase) ||
                    x.Key.Equals(QS_PAGE, StringComparison.OrdinalIgnoreCase) ||
                    x.Key.Equals(QS_PAGESIZE, StringComparison.OrdinalIgnoreCase)
                );
                var qb = new QueryBuilder(items);
                var qs = qb.ToQueryString();
                queryString = (qs.HasValue ? qs.Value + '&' :@"?");
            }
            else
            {
                queryString = @"?";
            }
            int maxPages = (int)Math.Ceiling((decimal)totalItems / pagingParameters.PageSize);
            var pager = new Pager((int)totalItems, pagingParameters.Page, pagingParameters.PageSize, maxPages);
            Self = root;
            First = pagingParameters.Page <= 1 ? null : root + queryString + QS_PAGESIZE + '=' + pagingParameters.PageSize + ('&' + QS_PAGE + '=' + '1');
            Next = pagingParameters.Page >= maxPages ? null : root + queryString + QS_PAGESIZE + '=' + pagingParameters.PageSize + ('&' + QS_PAGE + '=') + (pagingParameters.Page + 1);// +"&nextPageToken=" + pagingParameters.NextPageToken + ;
            Previous = pager.CurrentPage <= 1 ? null : root + queryString + QS_PAGESIZE + '=' + pagingParameters.PageSize + ('&' + QS_PAGE + '=') + (pager.CurrentPage - 1);
            Last = pager.CurrentPage >= maxPages ? null : root + queryString + QS_PAGESIZE + '=' + pagingParameters.PageSize + ('&' + QS_PAGE + '=') + maxPages;
            Pages = new Dictionary<int, string>();
            foreach (var p in pager.Pages)
            {
                Pages.Add(p, root + queryString + QS_PAGESIZE + '=' + pagingParameters.PageSize + ('&' + QS_PAGE + '=') + p);
            }
        }
        public string First { get; set; }
        public string Previous { get; set; }
        public Dictionary<int, string> Pages
        {
            get; set;
        }
        public string Next { get; set; }
        public string Last { get; set; }
    }
}
