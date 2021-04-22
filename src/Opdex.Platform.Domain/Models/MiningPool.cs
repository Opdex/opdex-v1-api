using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models
{
    public class MiningPool
    {
        public MiningPool(long id, long liquidityPoolId, string address, string rewardRate, ulong miningPeriodEndBlock)
        {
            Id = id;
            LiquidityPoolId = liquidityPoolId;
            Address = address;
            RewardRate = rewardRate;
            MiningPeriodEndBlock = miningPeriodEndBlock;
        }
        
        public MiningPool(long liquidityPoolId, string address, string rewardRate, ulong miningPeriodEndBlock)
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId));
            }

            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }
            
            if (!rewardRate.HasValue())
            {
                throw new ArgumentNullException(nameof(rewardRate));
            }

            LiquidityPoolId = liquidityPoolId;
            Address = address;
            RewardRate = rewardRate;
            MiningPeriodEndBlock = miningPeriodEndBlock;
        }
        
        public MiningPool(string address, string liquidityPoolAddress, string rewardRate, ulong miningPeriodEndBlock)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }
            
            if (!liquidityPoolAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(liquidityPoolAddress));
            }
            
            if (!rewardRate.HasValue())
            {
                throw new ArgumentNullException(nameof(rewardRate));
            }

            Address = address;
            LiquidityPoolAddress = liquidityPoolAddress;
            RewardRate = rewardRate;
            MiningPeriodEndBlock = miningPeriodEndBlock;
        }
        
        public long Id { get; }
        public long LiquidityPoolId { get; private set; }
        public string LiquidityPoolAddress { get; }
        public string Address { get; }
        public string RewardRate { get; }
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