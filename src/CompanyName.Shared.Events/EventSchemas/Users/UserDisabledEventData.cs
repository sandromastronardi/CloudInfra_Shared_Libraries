using System;

namespace CompanyName.Shared.Events.EventSchemas.Users
{
    public class UserDisabledEventData : DisabledEventBase
    {
        public UserDisabledEventData()
        {

        }
        public UserDisabledEventData(string id) : base(id)
        {
            Source = new Uri($"users/{id}", UriKind.Relative);
        }
    }
}