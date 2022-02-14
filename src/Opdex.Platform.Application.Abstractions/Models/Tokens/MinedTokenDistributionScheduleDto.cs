using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.Tokens;

public class MinedTokenDistributionScheduleDto
{
    public Address Vault { get; set; }

    public Address MiningGovernance { get; set; }

    public ulong NextDistributionBlock { get; set; }

    public MinedTokenDistributionItemDto[] History { get; set; }
}
