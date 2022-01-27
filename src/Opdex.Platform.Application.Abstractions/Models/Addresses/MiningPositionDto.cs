using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.Addresses;

public class MiningPositionDto
{
    public Address Address { get; set; }
    public FixedDecimal Amount { get; set; }
    public Address MiningPool { get; set; }
    public Address MiningToken { get; set; }
    public ulong CreatedBlock { get; set; }
    public ulong ModifiedBlock { get; set; }
}