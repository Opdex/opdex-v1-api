using Opdex.Platform.Common.Enums;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Tokens;

public class OwnershipTransferredEventDto : OwnershipEventDto
{
    public override TransactionEventType EventType => TransactionEventType.OwnershipTransferredEvent;
}
