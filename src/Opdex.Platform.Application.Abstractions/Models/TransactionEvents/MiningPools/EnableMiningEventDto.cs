using Opdex.Platform.Common.Enums;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningPools
{
    public class EnableMiningEventDto : TransactionEventDto
    {
        public string Amount { get; set; }
        public string RewardRate { get; set; }
        public ulong MiningPeriodEndBlock { get; set; }
        public override TransactionEventType EventType => TransactionEventType.EnableMiningEvent;
    }
}
