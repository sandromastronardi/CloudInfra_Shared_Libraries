using System;

namespace CompanyName.Shared.Events.EventSchemas.Users
{
    public class UserLoggedInEventData : EventBase
    {
        public UserLoggedInEventData() { }
        public UserLoggedInEventData(string id) : base(id)
        {
            Source = new Uri($"users/{id}", UriKind.Relative);
        }
    }
}