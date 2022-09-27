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
