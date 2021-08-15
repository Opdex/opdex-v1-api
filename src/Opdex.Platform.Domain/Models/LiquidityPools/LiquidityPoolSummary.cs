using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;
using System;

namespace Opdex.Platform.Domain.Models.LiquidityPools
{
    public class LiquidityPoolSummary : BlockAudit
    {
        public LiquidityPoolSummary(long liquidityPoolId, decimal liquidity, decimal volume, ulong stakingWeight, ulong lockedCrs, UInt256 lockedSrc, ulong createdBlock)
            : base(createdBlock, createdBlock)
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId), "LiquidityPoolId must be greater than 0.");
            }

            if (liquidity < 0m)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidity), "Liquidity must be greater than or equal to 0.");
            }

            if (volume < 0m)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidity), "Volume must be greater than or equal to 0.");
            }

            LiquidityPoolId = liquidityPoolId;
            Liquidity = liquidity;
            Volume = volume;
            StakingWeight = stakingWeight;
            LockedCrs = lockedCrs;
            LockedSrc = lockedSrc;
        }

        public LiquidityPoolSummary(long id, long liquidityPoolId, decimal liquidity, decimal volume, ulong stakingWeight, ulong lockedCrs, UInt256 lockedSrc,
                                    ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
        {
            Id = id;
            LiquidityPoolId = liquidityPoolId;
            Liquidity = liquidity;
            Volume = volume;
            StakingWeight = stakingWeight;
            LockedCrs = lockedCrs;
            LockedSrc = lockedSrc;
        }

        public long Id { get; }
        public long LiquidityPoolId { get; }
        public decimal Liquidity { get; }
        public decimal Volume { get; }
        public ulong StakingWeight { get; }
        public ulong LockedCrs { get; }
        public UInt256 LockedSrc { get; }
    }
}
