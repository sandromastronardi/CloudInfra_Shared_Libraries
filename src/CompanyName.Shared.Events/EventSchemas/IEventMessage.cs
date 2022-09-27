using System;

namespace CompanyName.Shared.Events.EventSchemas
{
    public interface IEventMessage
    {
        string Subject { get; }
        System.Guid Id { get; set; }
        Uri Source { get; set; }
        System.Guid UserId { get; set; }
        string OperationId { get; set; }
        string OperationParentId { get; set; }
    }
}
