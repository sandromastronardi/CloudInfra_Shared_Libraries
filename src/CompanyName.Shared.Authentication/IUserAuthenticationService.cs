// Copyright (c) Mastronardi Software (mastrosoft.com). All Rights Reserved.
// Licensed under Mozilla Public License, Version 2.0

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
#if(NETCOREAPP || NETSTANDARD)
using Microsoft.AspNetCore.Http;
#endif
namespace CompanyName.Shared.Authentication
{
    public interface IUserAuthenticationService
    {
        Task<Identity> GetUserAsync(HttpRequestMessage req);
        Task<Identity> GetUserAsync(HttpRequest req);
        Identity Identity { get; set; }
    }
}
