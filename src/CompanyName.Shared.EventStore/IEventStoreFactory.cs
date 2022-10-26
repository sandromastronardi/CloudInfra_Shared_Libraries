// Copyright (c) Mastronardi Software (mastrosoft.com). All Rights Reserved.
// Licensed under Mozilla Public License, Version 2.0

using Eveneum;

namespace CompanyName.Shared.EventStore
{
    public interface IEventStoreFactory
    {
        IEventStore GetEventStore();
    }
}