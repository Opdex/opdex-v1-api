using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionLogs
{
    public class NominationLog : TransactionLog
    {
        public NominationLog(dynamic log, string address, int sortOrder)
            : base(nameof(EnterStakingPoolLog), address, sortOrder)
        {
            string stakingPool = log?.stakingPool;
            string miningPool = log?.miningPool;
            string weight = log?.weight;

            if (!stakingPool.HasValue())
            {
                throw new ArgumentNullException(nameof(stakingPool));
            }
            
            if (!miningPool.HasValue())
            {
                throw new ArgumentNullException(nameof(miningPool));
            }
            
            if (!weight.HasValue())
            {
                throw new ArgumentNullException(nameof(weight));
            }

            StakingPool = stakingPool;
            MiningPool = miningPool;
            Weight = weight;
        }
        
        public string StakingPool { get; }
        public string MiningPool { get; }
        public string Weight { get; }
    }
}