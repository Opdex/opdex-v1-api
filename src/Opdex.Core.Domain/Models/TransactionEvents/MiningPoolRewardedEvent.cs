using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionEvents
{
    public class MiningPoolRewardedEvent : TransactionEvent
    {
        public MiningPoolRewardedEvent(dynamic log, string address, int sortOrder)
            : base(nameof(StakeEvent), address, sortOrder)
        {
            string stakingPool = log?.stakingPool;
            string miningPool = log?.miningPool;
            string amount = log?.amount;

            if (!stakingPool.HasValue())
            {
                throw new ArgumentNullException(nameof(stakingPool));
            }
            
            if (!miningPool.HasValue())
            {
                throw new ArgumentNullException(nameof(miningPool));
            }
            
            if (!amount.HasValue())
            {
                throw new ArgumentNullException(nameof(amount));
            }

            StakingPool = stakingPool;
            MiningPool = miningPool;
            Amount = amount;
        }
        
        public string StakingPool { get; }
        public string MiningPool { get; }
        public string Amount { get; }
    }
}