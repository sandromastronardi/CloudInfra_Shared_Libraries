using System;

namespace CompanyName.Shared.Events.EventSchemas.Users
{
    public class UserCreatedEventData : CreatedEventBase
    {
        public UserCreatedEventData() { }
        public UserCreatedEventData(string id) : base(id)
        {
            Source = new Uri($"users/{id}", UriKind.Relative);
        }
    }
}