using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.ODX
{
    public class MiningGovernanceNomination : BlockAudit
    {
        public MiningGovernanceNomination(long liquidityPoolId, long miningPoolId, bool isNominated, string weight, ulong createdBlock) : base(createdBlock)
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId), "Liqudity pool id must be greater than 0.");
            }

            if (miningPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(miningPoolId), "Mining pool id must be greater than 0.");
            }

            if (!weight.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(weight), "Weight must only contain numeric digits.");
            }

            LiquidityPoolId = liquidityPoolId;
            MiningPoolId = miningPoolId;
            IsNominated = isNominated;
            Weight = weight;
        }

        public MiningGovernanceNomination(long id, long liquidityPoolId, long miningPoolId, bool isNominated, string weight, ulong createdBlock, ulong modifiedBlock)
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
        public string Weight { get; }

        public void SetNominationStatus(bool status, ulong block)
        {
            IsNominated = status;
            SetModifiedBlock(block);
        }
    }
}