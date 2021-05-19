using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.ODX
{
    public class TokenDistribution
    {
        public TokenDistribution(long id, string vaultDistribution, string miningGovernanceDistribution, int periodIndex, ulong distributionBlock, ulong nextDistributionBlock,
            ulong createdBlock, ulong modifiedBlock)
        {
            Id = id;
            VaultDistribution = vaultDistribution;
            MiningGovernanceDistribution = miningGovernanceDistribution;
            PeriodIndex = periodIndex;
            DistributionBlock = distributionBlock;
            NextDistributionBlock = nextDistributionBlock;
            CreatedBlock = createdBlock;
            ModifiedBlock = modifiedBlock;
        }
        
        public TokenDistribution(string vaultDistribution, string miningGovernanceDistribution, int periodIndex, ulong distributionBlock, ulong nextDistributionBlock,
            ulong createdBlock, ulong modifiedBlock)
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
            
            if (createdBlock < 1)
            {
                throw new ArgumentNullException(nameof(createdBlock));
            }
            
            if (modifiedBlock < 1)
            {
                throw new ArgumentNullException(nameof(modifiedBlock));
            }

            VaultDistribution = vaultDistribution;
            MiningGovernanceDistribution = miningGovernanceDistribution;
            PeriodIndex = periodIndex;
            DistributionBlock = distributionBlock;
            NextDistributionBlock = nextDistributionBlock;
            CreatedBlock = createdBlock;
            ModifiedBlock = modifiedBlock;
        }

        public long Id { get; }
        public string VaultDistribution { get; }
        public string MiningGovernanceDistribution { get; }
        public int PeriodIndex { get; }
        public ulong DistributionBlock { get; }
        public ulong NextDistributionBlock { get; }
        public ulong CreatedBlock { get; }
        public ulong ModifiedBlock { get; }
    }
}