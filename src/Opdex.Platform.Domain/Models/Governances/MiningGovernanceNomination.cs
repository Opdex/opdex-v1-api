using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;
using System;

namespace Opdex.Platform.Domain.Models.Governances
{
    public class MiningGovernanceNomination : BlockAudit
    {
        public MiningGovernanceNomination(long liquidityPoolId, long miningPoolId, bool isNominated, UInt256 weight, ulong createdBlock) : base(createdBlock)
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId), "Liquidity pool id must be greater than 0.");
            }

            if (miningPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(miningPoolId), "Mining pool id must be greater than 0.");
            }

            LiquidityPoolId = liquidityPoolId;
            MiningPoolId = miningPoolId;
            IsNominated = isNominated;
            Weight = weight;
        }

        public MiningGovernanceNomination(long id, long liquidityPoolId, long miningPoolId, bool isNominated, UInt256 weight, ulong createdBlock, ulong modifiedBlock)
            : base(createdBlock, modifiedBlock)
        {
            Id = id;
            LiquidityPoolId = liquidityPoolId;
            MiningPoolId = miningPoolId;
            IsNominated = isNominated;
            Weight = weight;
        }

        public long Id { get; }
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
