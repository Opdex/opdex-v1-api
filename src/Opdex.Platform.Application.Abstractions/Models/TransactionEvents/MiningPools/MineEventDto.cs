namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningPools
{
    public class MineEventDto : TransactionEventDto
    {
        public string Miner { get; set; }
        public string Amount { get; set; }
        public string SubEventType { get; set; }
        public override TransactionEventType EventType => TransactionEventType.MineEvent;
    }
}
