using System;
using System.Net;

namespace CompanyName.Shared.Clients
{
    public class EmptyClientResult : ClientResult
    {
        [Obsolete("Use EmptyClientResult(HttpStatusCode) instead")]
        public EmptyClientResult() : base() { }
        public EmptyClientResult(HttpStatusCode statusCode) : base()
        {
            StatusCode = statusCode;
        }
    }
}