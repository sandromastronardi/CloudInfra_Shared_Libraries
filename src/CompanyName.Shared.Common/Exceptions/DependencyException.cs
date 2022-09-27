using System;
using System.Collections.Generic;

namespace CompanyName.Shared.Common.Exceptions
{
    public class DependencyException : Exception
    {
        public DependencyException(IEnumerable<Notification> notifications)
        {
            Notifications = notifications;
        }

        public IEnumerable<Notification> Notifications { get; private set; }
    }
}
