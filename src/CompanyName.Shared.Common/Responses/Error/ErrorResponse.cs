using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace CompanyName.Shared.Common.Responses.Error
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }

        public string StatusDescription { get; set; }
        public string Message { set; get; }
        public Notification[] Notifications { get; set; }

        private ErrorResponse()
        {

        }
        public ErrorResponse(HttpStatusCode statusCode):this((int)statusCode,statusCode.ToString())
        {

        }
        public ErrorResponse(HttpStatusCode statusCode, string statusDescription) : this((int)statusCode, statusDescription)
        {

        }
        public ErrorResponse(int statusCode, string statusDescription) : this()
        {
            StatusCode = statusCode;
            StatusDescription = statusDescription;
        }
        public ErrorResponse(HttpStatusCode statusCode, string statusDescription, string message):this((int)statusCode, statusDescription ?? statusCode.ToString(), message)
        {

        }
        public ErrorResponse(int statusCode, string statusDescription, string message)
            : this(statusCode, statusDescription)
        {
            Notifications = new[] { Notification.Error(message) };
        }
        public ErrorResponse(HttpStatusCode statusCode, string statusDescription, IEnumerable<Notification> notifications)
            : this((int)statusCode, statusDescription, notifications)
        {

        }

        public ErrorResponse(int statusCode, string statusDescription, IEnumerable<Notification> notifications)
            : this(statusCode, statusDescription)
        {
            Notifications = notifications.ToArray();
        }
    }
}