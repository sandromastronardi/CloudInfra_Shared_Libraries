// Copyright (c) Mastronardi Software (mastrosoft.com). All Rights Reserved.
// Licensed under Mozilla Public License, Version 2.0

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace CompanyName.Shared.Common
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum NotificationLevel
    {
        Info,
        Warning,
        Error
    }
}
