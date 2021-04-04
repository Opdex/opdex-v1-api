using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionEvents
{
    public class MiningPoolCreatedEvent : TransactionEvent
    {
        public MiningPoolCreatedEvent(dynamic log, string address, int sortOrder)
            : base(nameof(StakeEvent), address, sortOrder)
        {
            string stakingPool = log?.stakingPool;
            string miningPool = log?.miningPool;

            if (!stakingPool.HasValue())
            {
                throw new ArgumentNullException(nameof(stakingPool));
            }
            
            if (!miningPool.HasValue())
            {
                throw new ArgumentNullException(nameof(miningPool));
            }

            StakingPool = stakingPool;
            MiningPool = miningPool;
        }
        
        public string StakingPool { get; }
        public string MiningPool { get; }
    }
}