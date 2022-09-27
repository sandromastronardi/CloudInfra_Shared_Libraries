using Eveneum;

namespace CompanyName.Shared.EventStore
{
    public interface IEventStoreFactory
    {
        IEventStore GetEventStore();
    }
}