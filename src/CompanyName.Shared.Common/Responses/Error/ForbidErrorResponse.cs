// Copyright (c) Mastronardi Software (mastrosoft.com). All Rights Reserved.
// Licensed under Mozilla Public License, Version 2.0

using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace CompanyName.Shared.Common.Responses.Error
{
    public class ForbidErrorResponse : ErrorResponse
    {
        public ForbidErrorResponse()
            : base(403, HttpStatusCode.Forbidden.ToString())
        {
        }
        private static string ForbiddenMessage(string[] scopes)
        {
            return $"Access is forbidden" + ((scopes != null && scopes.Any()) ? ", you need one of these permissions: " + string.Join(", ", scopes) : string.Empty);
        }
        public ForbidErrorResponse(string[] scopes):base(403, HttpStatusCode.Forbidden.ToString(),ForbiddenMessage(scopes))
        {
        }

        public ForbidErrorResponse(string message)
            : base(403, HttpStatusCode.Forbidden.ToString(), message)
        {
        }

        public ForbidErrorResponse(IEnumerable<Notification> notifications)
            : base(403, HttpStatusCode.Forbidden.ToString(), notifications)
        {
        }
    }
}