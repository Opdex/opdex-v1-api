using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.Tokens;

public class TokenDto
{
    public ulong Id { get; set; }
    public Address Address { get; set; }
    public string Name { get; set; }
    public string Symbol { get; set; }
    public int Decimals { get; set; }
    public ulong Sats { get; set; }
    public FixedDecimal TotalSupply { get; set; }
    public IEnumerable<TokenAttributeType> Attributes { get; set; }
    public WrappedTokenDetailsDto WrappedToken { get; set; }
    public MinedTokenDistributionScheduleDto Distribution { get; set; }
    public ulong CreatedBlock { get; set; }
    public ulong ModifiedBlock { get; set; }
    public TokenSummaryDto Summary { get; set; }
}
