using Opdex.Platform.Common.Enums;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Tokens;

public class SetInterfluxCustodianEventDto : OwnershipEventDto
{
    public override TransactionEventType EventType => TransactionEventType.SetInterfluxCustodianEvent;
}
