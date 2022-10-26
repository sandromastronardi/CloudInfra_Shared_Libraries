// Copyright (c) Mastronardi Software (mastrosoft.com). All Rights Reserved.
// Licensed under Mozilla Public License, Version 2.0

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
