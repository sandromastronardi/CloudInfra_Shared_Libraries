using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CompanyName.Shared.Common.Api
{
    public class OrderByParameters
    {
        public enum Directions
        {
            Asc,
            Desc
        }
        public string[] OrderBy { get; set; }
        public Directions Direction { get; set; }

        public string GetOrderByQuery()
        {
            return OrderBy != null && OrderBy.Length > 0 ? string.Join(", ",OrderBy.Select(n => n + " " + Direction.ToString()).ToArray()) : string.Empty;
        }
    }
}
