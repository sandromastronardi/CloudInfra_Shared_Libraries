using System;

namespace CompanyName.Shared.Events.EventSchemas.Users
{
    public class UserEnabledEventData : EnabledEventBase
    {
        public UserEnabledEventData()
        {

        }
        public UserEnabledEventData(string id) : base(id)
        {
            Source = new Uri($"users/{id}", UriKind.Relative);
        }
    }
}