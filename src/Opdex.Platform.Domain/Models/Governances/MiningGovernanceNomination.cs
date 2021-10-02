using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;
using System;

namespace Opdex.Platform.Domain.Models.Governances
{
    public class MiningGovernanceNomination : BlockAudit
    {
        public MiningGovernanceNomination(long governanceId, long liquidityPoolId, long miningPoolId, bool isNominated, UInt256 weight, ulong createdBlock) : base(createdBlock)
        {
            if (governanceId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(governanceId), "Governance id must be greater than 0.");
            }

            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId), "Liquidity pool id must be greater than 0.");
            }

            if (miningPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(miningPoolId), "Mining pool id must be greater than 0.");
            }

            GovernanceId = governanceId;
            LiquidityPoolId = liquidityPoolId;
            MiningPoolId = miningPoolId;
            IsNominated = isNominated;
            Weight = weight;
        }

        public MiningGovernanceNomination(long id, long governanceId, long liquidityPoolId, long miningPoolId, bool isNominated, UInt256 weight, ulong createdBlock, ulong modifiedBlock)
            : base(createdBlock, modifiedBlock)
        {
            Id = id;
            GovernanceId = governanceId;
            LiquidityPoolId = liquidityPoolId;
            MiningPoolId = miningPoolId;
            IsNominated = isNominated;
            Weight = weight;
        }

        public long Id { get; }
        public long GovernanceId { get; }
        public long LiquidityPoolId { get; }
        public long MiningPoolId { get; }
        public bool IsNominated { get; private set; }
        public UInt256 Weight { get; private set; }

        public void SetStatus(bool status, ulong block)
        {
            IsNominated = status;
            SetModifiedBlock(block);
        }

        public void SetWeight(UInt256 weight, ulong block)
        {
            Weight = weight;
            SetModifiedBlock(block);
        }
    }
}
