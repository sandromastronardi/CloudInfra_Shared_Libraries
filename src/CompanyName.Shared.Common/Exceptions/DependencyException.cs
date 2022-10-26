// Copyright (c) Mastronardi Software (mastrosoft.com). All Rights Reserved.
// Licensed under Mozilla Public License, Version 2.0

using System;
using System.Collections.Generic;

namespace CompanyName.Shared.Common.Exceptions
{
    public class DependencyException : Exception
    {
        public DependencyException(IEnumerable<Notification> notifications)
        {
            Notifications = notifications;
        }

        public IEnumerable<Notification> Notifications { get; private set; }
    }
}
