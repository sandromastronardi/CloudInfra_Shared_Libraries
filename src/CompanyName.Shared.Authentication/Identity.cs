using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace CompanyName.Shared.Authentication
{
    public class Identity
    {
        public ClaimsPrincipal User { get; set; }
        public SecurityToken ValidatedToken { get; set; }
    }
}
