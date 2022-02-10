using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;
using System;

namespace Opdex.Platform.Domain.Models.Tokens;

public class TokenDistribution
{
    public TokenDistribution(ulong tokenId, UInt256 vaultDistribution, UInt256 miningGovernanceDistribution, int periodIndex, ulong distributionBlock,
                             ulong nextDistributionBlock)
    {
        if (tokenId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(tokenId), "Token Id must be greater than 0.");
        }
        if (distributionBlock < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(distributionBlock), "Distribution block must be greater than 0.");
        }

        if (nextDistributionBlock < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(nextDistributionBlock), "Next distribution block must be greater than 0.");
        }

        TokenId = tokenId;
        VaultDistribution = vaultDistribution;
        MiningGovernanceDistribution = miningGovernanceDistribution;
        PeriodIndex = periodIndex;
        DistributionBlock = distributionBlock;
        NextDistributionBlock = nextDistributionBlock;
    }

    public TokenDistribution(ulong id, ulong tokenId, UInt256 vaultDistribution, UInt256 miningGovernanceDistribution,
                             int periodIndex, ulong distributionBlock, ulong nextDistributionBlock)
    {
        Id = id;
        TokenId = tokenId;
        VaultDistribution = vaultDistribution;
        MiningGovernanceDistribution = miningGovernanceDistribution;
        PeriodIndex = periodIndex;
        DistributionBlock = distributionBlock;
        NextDistributionBlock = nextDistributionBlock;
    }

    public ulong Id { get; }
    public ulong TokenId { get; }
    public UInt256 VaultDistribution { get; }
    public UInt256 MiningGovernanceDistribution { get; }
    public int PeriodIndex { get; }
    public ulong DistributionBlock { get; }
    public ulong NextDistributionBlock { get; }
}
