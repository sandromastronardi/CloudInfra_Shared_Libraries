using System;

namespace CompanyName.Shared.Events.EventSchemas.Users
{
    public class UserUpdatedEventData : UpdatedEventBase
    {
        public UserUpdatedEventData() { }
        public UserUpdatedEventData(string id) : base(id)
        {
            Source = new Uri($"users/{id}", UriKind.Relative);
        }
    }
}