using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CompanyName.Shared.Authentication;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CompanyName.Shared.Auth0
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly ILogger _log;

        public UserAuthenticationService(ILogger<UserAuthenticationService> log)
        {
            _log = log;
        }
        public async Task<Identity> GetUserAsync(HttpRequestMessage req)
        {
            if (Identity == null)
            {
                var (user, token) = await req.AuthenticateAsync(_log);
                Identity = new Identity
                {
                    User = user,
                    ValidatedToken = token
                };
            }
            return Identity;
        }

        public async Task<Identity> GetUserAsync(HttpRequest req)
        {
            if (Identity == null)
            {
                var (user, token) = await req.AuthenticateAsync(_log);
                Identity = new Identity
                {
                    User = user,
                    ValidatedToken = token
                };
            }
            return Identity;
        }

        public Identity Identity { get; set; }
    }
}
