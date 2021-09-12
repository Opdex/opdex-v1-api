using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;
using System;

namespace Opdex.Platform.Domain.Models.MiningPools
{
    public class MiningPool : BlockAudit
    {

        public MiningPool(long liquidityPoolId, Address address, ulong createdBlock) : base(createdBlock)
        {
            if (address == Address.Empty)
            {
                throw new ArgumentNullException(nameof(address), "Address must be set.");
            }

            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId), "Liquidity pool id must be greater than 0.");
            }

            Address = address;
            LiquidityPoolId = liquidityPoolId;
            RewardPerBlock = UInt256.Zero;
            RewardPerLpt = UInt256.Zero;
            MiningPeriodEndBlock = 0;
        }


        public MiningPool(long id, long liquidityPoolId, Address address, UInt256 rewardPerBlock, UInt256 rewardPerLpt, ulong miningPeriodEndBlock,
            ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
        {
            Id = id;
            LiquidityPoolId = liquidityPoolId;
            Address = address;
            RewardPerBlock = rewardPerBlock;
            RewardPerLpt = rewardPerLpt;
            MiningPeriodEndBlock = miningPeriodEndBlock;
        }

        public long Id { get; }
        public long LiquidityPoolId { get; private set; }
        public Address Address { get; }
        public UInt256 RewardPerBlock { get; private set; }
        public UInt256 RewardPerLpt { get; }
        public ulong MiningPeriodEndBlock { get; private set; }

        public void SetLiquidityPoolId(long liquidityPoolId)
        {
            if (LiquidityPoolId < 1 && liquidityPoolId > 0)
            {
                LiquidityPoolId = liquidityPoolId;
            }
        }

        public void EnableMiningPool(EnableMiningLog log, ulong block)
        {
            RewardPerBlock = log.RewardRate;
            MiningPeriodEndBlock = log.MiningPeriodEndBlock;
            SetModifiedBlock(block);
        }
    }
}
