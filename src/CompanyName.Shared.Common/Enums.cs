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
