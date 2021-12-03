using Opdex.Platform.Common.Enums;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Markets;

public class ClaimPendingMarketOwnershipEventDto : OwnershipEventDto
{
    public override TransactionEventType EventType => TransactionEventType.ClaimPendingMarketOwnershipEvent;
}