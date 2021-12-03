using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningPools;

public abstract class MineEventDto : TransactionEventDto
{
    public Address Miner { get; set; }
    public FixedDecimal Amount { get; set; }
    public FixedDecimal TotalSupply { get; set; }
    public FixedDecimal MinerBalance { get; set; }
}