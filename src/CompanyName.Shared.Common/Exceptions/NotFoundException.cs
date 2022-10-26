// Copyright (c) Mastronardi Software (mastrosoft.com). All Rights Reserved.
// Licensed under Mozilla Public License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyName.Shared.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }

        public NotFoundException(IEnumerable<Notification> notifications)
        {
            Notifications = notifications;
        }

        public IEnumerable<Notification> Notifications { get; private set; }
    }
}
