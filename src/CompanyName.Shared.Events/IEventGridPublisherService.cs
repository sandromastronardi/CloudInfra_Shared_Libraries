// Copyright (c) Mastronardi Software (mastrosoft.com). All Rights Reserved.
// Licensed under Mozilla Public License, Version 2.0

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;

namespace CompanyName.Shared.Events
{
    public interface IEventGridPublisherService
    {
        [Obsolete("Use PostEventAsync instead")]
        Task PostEventGridEventAsync<T>(string type, string subject, T payload, string topic = null);
        Task PostEventAsync<T>(string type, T payload, string topic = null) where T : EventSchemas.IEventMessage;
        Task PostEventAsync<T>(string type, string subject, T payload, string topic = null);
    }
}