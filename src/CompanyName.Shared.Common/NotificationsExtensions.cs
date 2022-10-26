// Copyright (c) Mastronardi Software (mastrosoft.com). All Rights Reserved.
// Licensed under Mozilla Public License, Version 2.0

using CompanyName.Shared.Common.Responses.Error;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace CompanyName.Shared.Common
{
    public static class NotificationsExtensions
    {
        public static bool IsSuccessful(this Notification[] notifications, bool ignoreWarnings)
        {
            return notifications
                .All(n => n.Level != NotificationLevel.Error) &&
                   (ignoreWarnings || notifications.All(n => n.Level != NotificationLevel.Warning));
        }

        public static bool IsSuccessful(this IEnumerable<Notification> notifications, bool ignoreWarnings)
        {
            return notifications
                .All(n => n.Level != NotificationLevel.Error) &&
                   (ignoreWarnings || notifications.All(n => n.Level != NotificationLevel.Warning));
        }

        public static bool IsSuccessful(this List<Notification> notifications, bool ignoreWarnings)
        {
            return notifications
                .All(n => n.Level != NotificationLevel.Error) &&
                   (ignoreWarnings || notifications.All(n => n.Level != NotificationLevel.Warning));
        }

        public static ErrorResponse GetValidatorErrorResponse(this IEnumerable<ValidationResult> validationResults)
        {
            return new ValidationFailedErrorResponse(validationResults.Any(n => n.MemberNames.Any())?"Something went wrong with the values you provided":null, validationResults);
        }
    }
}
