using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionLogs
{
    public class CollectStakingRewardsLog : TransactionLog
    {
        public CollectStakingRewardsLog(dynamic log, string address, int sortOrder)
            : base(nameof(EnterStakingPoolLog), address, sortOrder)
        {
            string staker = log?.staker;
            string amount = log?.amount;
            string reward = log?.reward;

            if (!staker.HasValue())
            {
                throw new ArgumentNullException(nameof(staker));
            }
            
            if (!amount.HasValue())
            {
                throw new ArgumentNullException(nameof(amount));
            }
            
            if (!reward.HasValue())
            {
                throw new ArgumentNullException(nameof(reward));
            }

            Staker = staker;
            Amount = amount;
            Reward = reward;
        }
        
        public string Staker { get; }
        public string Amount { get; }
        public string Reward { get; }
    }
}