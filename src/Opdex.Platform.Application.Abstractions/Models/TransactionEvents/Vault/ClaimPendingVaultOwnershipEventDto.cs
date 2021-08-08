using Opdex.Platform.Common.Enums;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Vault
{
    public class ClaimPendingVaultOwnershipEventDto : OwnershipEventDto
    {
        public override TransactionEventType EventType => TransactionEventType.ClaimPendingVaultOwnershipEvent;
    }
}
