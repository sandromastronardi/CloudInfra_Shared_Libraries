using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CompanyName.Shared.Auth0
{
    public class Auth0UnauthorizedResult : UnauthorizedObjectResult
    {
        public Auth0UnauthorizedResult() : base(null) { }
        public Auth0UnauthorizedResult(object o):base(o)
        {

        }
        public override void OnFormatting(ActionContext context)
        {
            base.OnFormatting(context);
            context.HttpContext.Response.Headers.Add("WWW-Authenticate",new AuthenticationHeaderValue("Bearer", "token_type=\"JWT\"").ToString());
        }
    }
}
