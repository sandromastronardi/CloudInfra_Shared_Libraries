using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.Shared.Events.EventSchemas
{
    public abstract class EventBase
    {
        protected string _subject;
        public EventBase() { }
        public EventBase(string id)
        {
            Guid guidId;
            if (id != null && Guid.TryParse(id, out guidId))
            {
                Id = guidId;
            }
        }
        public Guid Id { get; set; }
        public Uri Source { get; set; }
        public Guid UserId { get; set; }
        public string OperationId { get; set; }
        public string OperationParentId { get; set; }
    }
}
