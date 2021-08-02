namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools
{
    public class CollectStakingRewardsEventDto : TransactionEventDto
    {
        public string Staker { get; set; }
        public string Amount { get; set; }
        public override TransactionEventType EventType => TransactionEventType.CollectStakingRewardsEvent;
    }
}
