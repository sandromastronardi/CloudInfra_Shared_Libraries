// Copyright (c) Mastronardi Software (mastrosoft.com). All Rights Reserved.
// Licensed under Mozilla Public License, Version 2.0

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CompanyName.Shared.Events
{
    public interface IEventGridSubscriberService
    {
        Task<IActionResult> HandleSubscriptionValidationEvent(HttpRequest req);
        Task<IActionResult> HandleSubscriptionValidationEvent(HttpRequestMessage req);
        (EventGridEvent eventGridEvent, string userId, string itemId) DeconstructEventGridMessage(HttpRequest req);
    }
}