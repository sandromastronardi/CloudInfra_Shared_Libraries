// Copyright (c) Mastronardi Software (mastrosoft.com). All Rights Reserved.
// Licensed under Mozilla Public License, Version 2.0

using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace CompanyName.Shared.Authentication
{
    public static class AuthorisationExtensions
    {
        public static Guid? GetId(this ClaimsPrincipal user)
        {
            var id = user.FindFirst(n => n.Type == "https://CompanyName.net/uuid")?.Value;
            return !string.IsNullOrEmpty(id) ? new Guid(id) : default(Guid?);
        }
    }
}
