// Copyright (c) Mastronardi Software (mastrosoft.com). All Rights Reserved.
// Licensed under Mozilla Public License, Version 2.0

namespace CompanyName.Shared.Common
{
    public class Notification {
        public NotificationLevel Level { get; set; }
        public string Message { get; set; }

        public Notification(NotificationLevel level, string message)
        {
            Level = level;
            Message = message;
        }

        public static Notification Error(string message) => new Notification(NotificationLevel.Error, message);

        public static Notification Warning(string message) => new Notification(NotificationLevel.Warning, message);

        public static Notification Info(string message) => new Notification(NotificationLevel.Info, message);

    }
}