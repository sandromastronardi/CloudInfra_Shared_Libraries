using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CompanyName.Shared.Auth0
{
    public sealed class AuthenticationExpectedException : ExpectedException
    {
        public AuthenticationExpectedException(string message = "Authentication failed", Exception exception = null)
            : base(HttpStatusCode.Forbidden, message, exception)
        {
        }

        public AuthenticationExpectedException(Exception exception = null)
    : base(HttpStatusCode.Forbidden, "Authentication failed", exception)
        {
        }

        protected override void ApplyResponseDetails(HttpResponseMessage response)
        {
            response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Bearer", "token_type=\"JWT\""));
        }
    }
}
