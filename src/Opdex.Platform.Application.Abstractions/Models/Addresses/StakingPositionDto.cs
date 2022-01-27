using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.Addresses;

public class StakingPositionDto
{
    public Address Address { get; set; }
    public FixedDecimal Amount { get; set; }
    public Address LiquidityPool { get; set; }
    public Address StakingToken { get; set; }
    public ulong CreatedBlock { get; set; }
    public ulong ModifiedBlock { get; set; }
}