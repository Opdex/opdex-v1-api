using Opdex.Platform.Common.Enums;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools
{
    public class StopStakingEventDto : StakeEventDto
    {
        public override TransactionEventType EventType => TransactionEventType.StopStakingEvent;
    }
}
