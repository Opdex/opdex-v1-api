using Opdex.Platform.Common.Enums;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Governances
{
    public class RewardMiningPoolEventDto : TransactionEventDto
    {
        public string StakingPool { get; set; }
        public string MiningPool { get; set; }
        public string Amount { get; set; }
        public override TransactionEventType EventType => TransactionEventType.RewardMiningPoolEvent;
    }
}