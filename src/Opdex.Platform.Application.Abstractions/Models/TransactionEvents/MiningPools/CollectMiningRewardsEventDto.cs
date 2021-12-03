using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningPools;

public class CollectMiningRewardsEventDto : TransactionEventDto
{
    public Address Miner { get; set; }
    public FixedDecimal Amount { get; set; }
    public override TransactionEventType EventType => TransactionEventType.CollectMiningRewardsEvent;
}