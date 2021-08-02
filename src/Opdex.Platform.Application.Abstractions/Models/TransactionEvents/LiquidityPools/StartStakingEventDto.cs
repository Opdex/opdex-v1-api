namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools
{
    public class StartStakingEventDto : StakeEventDto
    {
        public override TransactionEventType EventType => TransactionEventType.StartStakingEvent;
    }
}
