// Copyright (c) Mastronardi Software (mastrosoft.com). All Rights Reserved.
// Licensed under Mozilla Public License, Version 2.0

using Eveneum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompanyName.Shared.EventStore
{
    public interface IEventStoreRepository
    {
        Task<Response> AddEventAsync(string streamId, object body, object metadata = null);
        Task<Response> AddSnapshot(string streamId, ulong version, object snapshot);
        Task<Response> DeleteSnapshots(string streamId, ulong version);
        Task<Stream?> GetStream(string streamId);
        Task<List<StreamHeader>> GetStreamHeaders();
        Task<DeleteResponse> DeleteStream(string streamId);
    }
}