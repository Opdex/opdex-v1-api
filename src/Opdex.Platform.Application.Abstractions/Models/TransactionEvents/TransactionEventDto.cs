using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents
{
    public abstract class TransactionEventDto
    {
        public ulong Id { get; set; }
        public ulong TransactionId { get; set; }
        public Address Contract { get; set; }
        public int SortOrder { get; set; }
        public abstract TransactionEventType EventType { get; }
    }
}
