// Copyright (c) Mastronardi Software (mastrosoft.com). All Rights Reserved.
// Licensed under Mozilla Public License, Version 2.0

using System;
using System.Net;

namespace CompanyName.Shared.Clients
{
    public class EmptyClientResult : ClientResult
    {
        [Obsolete("Use EmptyClientResult(HttpStatusCode) instead")]
        public EmptyClientResult() : base() { }
        public EmptyClientResult(HttpStatusCode statusCode) : base()
        {
            StatusCode = statusCode;
        }
    }
}