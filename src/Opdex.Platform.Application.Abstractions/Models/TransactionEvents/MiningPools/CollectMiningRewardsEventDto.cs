namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningPools
{
    public class CollectMiningRewardsEventDto : TransactionEventDto
    {
        public string Miner { get; set; }
        public string Amount { get; set; }
        public override TransactionEventType EventType => TransactionEventType.CollectMiningRewardsEvent;
    }
}
