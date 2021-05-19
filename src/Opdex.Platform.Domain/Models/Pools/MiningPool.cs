using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;

namespace Opdex.Platform.Domain.Models.Pools
{
    public class MiningPool : BlockAudit
    {
        
        public MiningPool(long liquidityPoolId, string address, ulong createdBlock) : base(createdBlock)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }
            
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId));
            }

            Address = address;
            LiquidityPoolId = liquidityPoolId;
            RewardPerBlock = "0";
            RewardPerLpt = "0";
            MiningPeriodEndBlock = 0;
        }
        
        
        public MiningPool(long id, long liquidityPoolId, string address, string rewardPerBlock, string rewardPerLpt, ulong miningPeriodEndBlock, 
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
        public string Address { get; }
        public string RewardPerBlock { get; private set; }
        public string RewardPerLpt { get; }
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