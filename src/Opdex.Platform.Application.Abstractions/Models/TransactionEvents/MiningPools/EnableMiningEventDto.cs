using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningPools
{
    public class EnableMiningEventDto : TransactionEventDto
    {
        public FixedDecimal Amount { get; set; }
        public FixedDecimal RewardRate { get; set; }
        public ulong MiningPeriodEndBlock { get; set; }
        public override TransactionEventType EventType => TransactionEventType.EnableMiningEvent;
    }
}
