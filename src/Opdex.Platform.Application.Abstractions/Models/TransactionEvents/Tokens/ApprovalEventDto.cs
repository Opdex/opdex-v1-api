using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Tokens
{
    public class ApprovalEventDto : TransactionEventDto
    {
        public Address Owner { get; set; }
        public Address Spender { get; set; }
        public FixedDecimal Amount { get; set; }
        public override TransactionEventType EventType => TransactionEventType.ApprovalEvent;
    }
}
