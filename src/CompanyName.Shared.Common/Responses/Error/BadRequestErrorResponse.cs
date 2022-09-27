using System.Collections.Generic;
using System.Net;

namespace CompanyName.Shared.Common.Responses.Error
{
    public class BadRequestErrorResponse : ErrorResponse
    {
        public BadRequestErrorResponse()
            : base(400, HttpStatusCode.BadRequest.ToString())
        {
        }

        public BadRequestErrorResponse(string message)
            : base(400, HttpStatusCode.BadRequest.ToString(), message)
        {
        }

        public BadRequestErrorResponse(IEnumerable<Notification> notifications)
            : base(400, HttpStatusCode.BadRequest.ToString(), notifications)
        {
        }
    }

    public class ConflictErrorResponse : ErrorResponse
    {
        public ConflictErrorResponse()
            : base(409, HttpStatusCode.Conflict.ToString())
        {
        }

        public ConflictErrorResponse(string message)
            : base(409, HttpStatusCode.Conflict.ToString(), message)
        {
        }

        public ConflictErrorResponse(IEnumerable<Notification> notifications)
            : base(409, HttpStatusCode.Conflict.ToString(), notifications)
        {
        }
    }
}