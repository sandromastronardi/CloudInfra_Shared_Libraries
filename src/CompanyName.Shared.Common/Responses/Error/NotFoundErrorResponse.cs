using System.Net;

namespace CompanyName.Shared.Common.Responses.Error
{
    public class NotFoundErrorResponse : ErrorResponse
    {
        public NotFoundErrorResponse()
            : base(404, HttpStatusCode.NotFound.ToString())
        {
        }

        public NotFoundErrorResponse(string message)
            : base(404, HttpStatusCode.NotFound.ToString(), message)
        {
        }

        public NotFoundErrorResponse(Notification[] notifications)
            : base(404, HttpStatusCode.NotFound.ToString(), notifications)
        {
        }
    }
}