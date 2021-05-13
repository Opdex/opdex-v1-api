using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.Pools
{
    public class MiningPool
    {
        public MiningPool(long id, long liquidityPoolId, string address, string rewardPerBlock, string rewardPerLpt, ulong miningPeriodEndBlock)
        {
            Id = id;
            LiquidityPoolId = liquidityPoolId;
            Address = address;
            RewardPerBlock = rewardPerBlock;
            RewardPerLpt = rewardPerLpt;
            MiningPeriodEndBlock = miningPeriodEndBlock;
        }
        
        public MiningPool(long liquidityPoolId, string address, string rewardPerBlock, string rewardPerLpt, ulong miningPeriodEndBlock)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }
            
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId));
            }
            
            if (!rewardPerBlock.IsNumeric())
            {
                throw new ArgumentNullException(nameof(rewardPerBlock));
            }
            
            if (!rewardPerLpt.IsNumeric())
            {
                throw new ArgumentNullException(nameof(rewardPerBlock));
            }
            
            if (miningPeriodEndBlock < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(miningPeriodEndBlock));
            }

            Address = address;
            LiquidityPoolId = liquidityPoolId;
            RewardPerBlock = rewardPerBlock;
            RewardPerLpt = rewardPerBlock;
            MiningPeriodEndBlock = miningPeriodEndBlock;
        }
        
        public long Id { get; }
        public long LiquidityPoolId { get; private set; }
        public string Address { get; }
        public string RewardPerBlock { get; }
        public string RewardPerLpt { get; }
        public ulong MiningPeriodEndBlock { get; }

        public void SetLiquidityPoolId(long liquidityPoolId)
        {
            if (LiquidityPoolId < 1 && liquidityPoolId > 0)
            {
                LiquidityPoolId = liquidityPoolId;
            }
        }
    }
}