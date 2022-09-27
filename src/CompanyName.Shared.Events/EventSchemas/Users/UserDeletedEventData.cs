using System;

namespace CompanyName.Shared.Events.EventSchemas.Users
{
    public class UserDeletedEventData : DeletedEventBase
    {
        public UserDeletedEventData() { }
        public UserDeletedEventData(string id) : base(id)
        {
            Source = new Uri($"users/{id}", UriKind.Relative);
        }
    }
}