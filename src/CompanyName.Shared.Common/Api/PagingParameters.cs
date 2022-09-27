//using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CompanyName.Shared.Common.Api
{
    public class PagingParameters
    {
        [Range(1, double.PositiveInfinity)]
        public int Page { get; set; } = 1;
        [Range(1,250)]
        public int PageSize { get; set; } = 25;
        public string NextPageToken { get; set; }
    }
}
