using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using System;

namespace Opdex.Platform.Domain.Models.LiquidityPools
{
    public class LiquidityPoolSummary : BlockAudit
    {
        public LiquidityPoolSummary(long liquidityPoolId, decimal liquidity, decimal volume, ulong stakingWeight, ulong lockedCrs, UInt256 lockedSrc, ulong createdBlock)
            : this(liquidityPoolId, createdBlock)
        {
            if (liquidity < 0m)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidity), "LiquidityUsd must be greater than or equal to 0.");
            }

            if (volume < 0m)
            {
                throw new ArgumentOutOfRangeException(nameof(volume), "VolumeUsd must be greater than or equal to 0.");
            }

            LiquidityUsd = liquidity;
            VolumeUsd = volume;
            StakingWeight = stakingWeight;
            LockedCrs = lockedCrs;
            LockedSrc = lockedSrc;
        }

        public LiquidityPoolSummary(long liquidityPoolId, ulong createdBlock) : base(createdBlock, createdBlock)
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId), "LiquidityPoolId must be greater than 0.");
            }

            LiquidityPoolId = liquidityPoolId;
        }

        public LiquidityPoolSummary(long id, long liquidityPoolId, decimal liquidity, decimal volume, ulong stakingWeight, ulong lockedCrs, UInt256 lockedSrc,
                                    ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
        {
            Id = id;
            LiquidityPoolId = liquidityPoolId;
            LiquidityUsd = liquidity;
            VolumeUsd = volume;
            StakingWeight = stakingWeight;
            LockedCrs = lockedCrs;
            LockedSrc = lockedSrc;
        }

        public long Id { get; }
        public long LiquidityPoolId { get; }
        public decimal LiquidityUsd { get; private set; }
        public decimal VolumeUsd { get; private set; }
        public ulong StakingWeight { get; private set; }
        public ulong LockedCrs { get; private set; }
        public UInt256 LockedSrc { get; private set; }

        public void Update(LiquidityPoolSnapshot snapshot, ulong blockHeight)
        {
            LiquidityUsd = snapshot.Reserves.Usd;
            VolumeUsd = snapshot.Volume.Usd;
            StakingWeight = (ulong)snapshot.Staking.Weight;
            LockedCrs = snapshot.Reserves.Crs;
            LockedSrc = snapshot.Reserves.Src;
            SetModifiedBlock(blockHeight);
        }
    }
}
