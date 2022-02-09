using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.Abstractions.Models.Tokens;

public class WrappedTokenDetailsDto
{
    public Address Custodian { get; set; }
    public ExternalChainType Chain { get; set; }
    public string Address { get; set; }
    public bool Validated { get; set; }
    public ulong CreatedBlock { get; set; }
    public ulong ModifiedBlock { get; set; }
}
