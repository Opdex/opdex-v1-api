using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Governances
{
    public class RewardMiningPoolEventDto : TransactionEventDto
    {
        public Address StakingPool { get; set; }
        public Address MiningPool { get; set; }
        public FixedDecimal Amount { get; set; }
        public override TransactionEventType EventType => TransactionEventType.RewardMiningPoolEvent;
    }
}
