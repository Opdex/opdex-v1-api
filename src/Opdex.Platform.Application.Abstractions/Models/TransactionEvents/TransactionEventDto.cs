using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents
{
    public abstract class TransactionEventDto
    {
        public long Id { get; set; }
        public long TransactionId { get; set; }
        public Address Contract { get; set; }
        public int SortOrder { get; set; }
        public abstract TransactionEventType EventType { get; }
    }
}
