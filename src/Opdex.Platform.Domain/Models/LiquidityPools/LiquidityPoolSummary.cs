using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using System;

namespace Opdex.Platform.Domain.Models.LiquidityPools
{
    public class LiquidityPoolSummary : BlockAudit
    {
        public LiquidityPoolSummary(ulong liquidityPoolId, ulong createdBlock) : base(createdBlock, createdBlock)
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId), "LiquidityPoolId must be greater than 0.");
            }

            LiquidityPoolId = liquidityPoolId;
        }

        public LiquidityPoolSummary(ulong id, ulong liquidityPoolId, decimal liquidity, decimal volume, ulong stakingWeight, ulong lockedCrs, UInt256 lockedSrc,
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

        public ulong Id { get; }
        public ulong LiquidityPoolId { get; }
        public decimal LiquidityUsd { get; private set; }
        public decimal VolumeUsd { get; private set; }
        public ulong StakingWeight { get; private set; }
        public ulong LockedCrs { get; private set; }
        public UInt256 LockedSrc { get; private set; }

        public void Update(LiquidityPoolSnapshot snapshot, ulong blockHeight)
        {
            LiquidityUsd = snapshot.Reserves.Usd.Close;
            VolumeUsd = snapshot.Volume.Usd;
            StakingWeight = (ulong)snapshot.Staking.Weight;
            LockedCrs = snapshot.Reserves.Crs;
            LockedSrc = snapshot.Reserves.Src;
            SetModifiedBlock(blockHeight);
        }
    }
}
