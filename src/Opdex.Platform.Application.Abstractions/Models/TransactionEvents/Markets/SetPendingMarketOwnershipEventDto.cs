using Opdex.Platform.Common.Enums;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Markets;

public class SetPendingMarketOwnershipEventDto : OwnershipEventDto
{
    public override TransactionEventType EventType => TransactionEventType.SetPendingMarketOwnershipEvent;
}