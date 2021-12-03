using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools;

public class CollectStakingRewardsEventDto : TransactionEventDto
{
    public Address Staker { get; set; }
    public FixedDecimal Amount { get; set; }
    public override TransactionEventType EventType => TransactionEventType.CollectStakingRewardsEvent;
}