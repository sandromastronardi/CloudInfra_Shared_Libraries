// Copyright (c) Mastronardi Software (mastrosoft.com). All Rights Reserved.
// Licensed under Mozilla Public License, Version 2.0

using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.Shared.Events
{
    public class EventGridEvent : EventGridEvent<object>
    {
    }

    public class EventGridEvent<T>
    {
        public string Topic { get; set; }
        public string Id { get; set; }
        public string EventType { get; set; }
        public string Subject { get; set; }
        public DateTime EventTime { get; set; }

        public T Data { get; set; }
    }
}
