using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Blocks;
using System;

namespace Opdex.Platform.Domain.Models.Tokens
{
    public class TokenDistribution : BlockAudit
    {
        public TokenDistribution(long tokenId, string vaultDistribution, string miningGovernanceDistribution, int periodIndex, ulong distributionBlock,
                                 ulong nextDistributionBlock, ulong createdBlock) : base(createdBlock)
        {
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId), "Token Id must be greater than 0.");
            }

            if (!vaultDistribution.IsNumeric())
            {
                throw new ArgumentException("Vault distribution must only contain numeric digits.", nameof(vaultDistribution));
            }

            if (!miningGovernanceDistribution.IsNumeric())
            {
                throw new ArgumentException("Mining governance distribution must only contain numeric digits.", nameof(miningGovernanceDistribution));
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

        public TokenDistribution(long id, long tokenId, string vaultDistribution, string miningGovernanceDistribution, int periodIndex, ulong distributionBlock,
                                 ulong nextDistributionBlock, ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
        {
            Id = id;
            TokenId = tokenId;
            VaultDistribution = vaultDistribution;
            MiningGovernanceDistribution = miningGovernanceDistribution;
            PeriodIndex = periodIndex;
            DistributionBlock = distributionBlock;
            NextDistributionBlock = nextDistributionBlock;
        }

        public long Id { get; }
        public long TokenId { get; }
        public string VaultDistribution { get; }
        public string MiningGovernanceDistribution { get; }
        public int PeriodIndex { get; }
        public ulong DistributionBlock { get; }
        public ulong NextDistributionBlock { get; }
    }
}
