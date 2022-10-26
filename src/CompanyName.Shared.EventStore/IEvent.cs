// Copyright (c) Mastronardi Software (mastrosoft.com). All Rights Reserved.
// Licensed under Mozilla Public License, Version 2.0

using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.Shared.EventStore
{
    public interface IEvent<T>
    {
        void Apply(T target);
    }
}
