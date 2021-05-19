using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.ODX
{
    public class TokenDistribution : BlockAudit
    {
        public TokenDistribution(string vaultDistribution, string miningGovernanceDistribution, int periodIndex, ulong distributionBlock, ulong nextDistributionBlock,
            ulong createdBlock) : base(createdBlock)
        {
            if (!vaultDistribution.IsNumeric())
            {
                throw new ArgumentException(nameof(vaultDistribution));
            }
            
            if (!miningGovernanceDistribution.IsNumeric())
            {
                throw new ArgumentException(nameof(miningGovernanceDistribution));
            }

            if (distributionBlock < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(distributionBlock));
            }
            
            if (nextDistributionBlock < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(nextDistributionBlock));
            }

            VaultDistribution = vaultDistribution;
            MiningGovernanceDistribution = miningGovernanceDistribution;
            PeriodIndex = periodIndex;
            DistributionBlock = distributionBlock;
            NextDistributionBlock = nextDistributionBlock;
        }

        public TokenDistribution(long id, string vaultDistribution, string miningGovernanceDistribution, int periodIndex, ulong distributionBlock, ulong nextDistributionBlock,
            ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
        {
            Id = id;
            VaultDistribution = vaultDistribution;
            MiningGovernanceDistribution = miningGovernanceDistribution;
            PeriodIndex = periodIndex;
            DistributionBlock = distributionBlock;
            NextDistributionBlock = nextDistributionBlock;
        }
        
        public long Id { get; }
        public string VaultDistribution { get; }
        public string MiningGovernanceDistribution { get; }
        public int PeriodIndex { get; }
        public ulong DistributionBlock { get; }
        public ulong NextDistributionBlock { get; }
    }
}