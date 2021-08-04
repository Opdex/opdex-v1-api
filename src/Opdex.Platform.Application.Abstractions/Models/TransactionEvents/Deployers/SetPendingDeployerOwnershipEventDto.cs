using Opdex.Platform.Common.Enums;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Deployers
{
    public class SetPendingDeployerOwnershipEventDto : OwnershipEventDto
    {
        public override TransactionEventType EventType => TransactionEventType.SetPendingDeployerOwnershipEvent;
    }
}
