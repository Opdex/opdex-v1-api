using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.Tokens;

public class MinedTokenDistributionItemDto
{
    public FixedDecimal Vault { get; set; }

    public FixedDecimal MiningGovernance { get; set; }

    public ulong Block { get; set; }
}
