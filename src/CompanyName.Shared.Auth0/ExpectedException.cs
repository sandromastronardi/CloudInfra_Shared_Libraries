using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
//using System.Web.Http.Results;

namespace CompanyName.Shared.Auth0
{
    public class ExpectedException : Exception
    {
        public ExpectedException(HttpStatusCode code, string message = "", Exception exception = null)
            : base(message,exception)
        {
            Code = code;
        }

        public HttpStatusCode Code { get; }

        public HttpResponseMessage CreateErrorResponseMessage(HttpRequestMessage request)
        {
            //var result = request.CreateErrorResponse(Code, Message);
            var result = new HttpResponseMessage(Code) { RequestMessage = request, ReasonPhrase = Message };
            ApplyResponseDetails(result);
            return result;
        }
        public IActionResult CreateErrorActionResult(HttpRequestMessage request)
        {
            IActionResult response;
            //we want a 303 with the ability to set location
            HttpResponseMessage responseMsg = CreateErrorResponseMessage(request);
            response = new Microsoft.AspNetCore.Mvc.AcceptedResult("", null);// (StatusCodes.Status500InternalServerError);//InternalServerErrorResult(request);
            
            //result.Request.Content.

            //response = new ResponseMessageResult(responseMsg);
            return response;
        }

        protected virtual void ApplyResponseDetails(HttpResponseMessage response) { }
    }
}
