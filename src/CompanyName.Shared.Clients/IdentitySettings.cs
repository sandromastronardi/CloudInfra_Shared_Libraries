// Copyright (c) Mastronardi Software (mastrosoft.com). All Rights Reserved.
// Licensed under Mozilla Public License, Version 2.0

using System;

namespace CompanyName.Shared.Clients
{
    public class IdentitySettings
    {
        public IdentitySettings()
        {
            switch (Environment.GetEnvironmentVariable("AppEnvironment")?.ToLowerInvariant())
            {
                case "dev":
                case "development":
                    BaseUrl = "https://identity.companyname-dev.net";
                    Audience = "https://api.companyname-dev.net";
                    break;
                case "test":
                case "testing":
                    BaseUrl = "https://identity.companyname-test.net";
                    Audience = "https://api.companyname-test.net";
                    break;
                case "acc":
                case "acceptance":
                case "staging":
                    BaseUrl = "https://identity.companyname-staging.net";
                    Audience = "https://api.companyname-staging.net";
                    break;
                default:
                    BaseUrl = "https://identity.companyname.com";
                    Audience = "https://api.companyname.com";
                    break;

            }
        }
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Audience { get; set; }
        public bool IsDevice { get; set; }
        public string Scope { get; set; }
    }
}