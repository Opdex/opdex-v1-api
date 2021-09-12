using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Tokens
{
    public class DistributionEventDto : TransactionEventDto
    {
        public FixedDecimal VaultAmount { get; set; }
        public FixedDecimal GovernanceAmount { get; set; }
        public uint PeriodIndex { get; set; }
        public FixedDecimal TotalSupply { get; set; }
        public ulong NextDistributionBlock { get; set; }
        public override TransactionEventType EventType => TransactionEventType.DistributionEvent;
    }
}
