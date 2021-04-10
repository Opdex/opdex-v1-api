using System;
using Newtonsoft.Json;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionLogs
{
    public class CollectStakingRewardsLog : TransactionLog
    {
        public CollectStakingRewardsLog(dynamic log, string address, int sortOrder)
            : base(nameof(CollectStakingRewardsLog), address, sortOrder)
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
        
        public CollectStakingRewardsLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(nameof(CollectStakingRewardsLog), id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Staker = logDetails.Staker;
            Amount = logDetails.Amount;
            Reward = logDetails.Reward;
        }
        
        public string Staker { get; }
        public string Amount { get; }
        public string Reward { get; }
        
        private struct LogDetails
        {
            public string Staker { get; set; }
            public string Amount { get; set; }
            public string Reward { get; set; }
        }

        private static LogDetails DeserializeLogDetails(string details)
        {
            return JsonConvert.DeserializeObject<LogDetails>(details);
        }

        public override string SerializeLogDetails()
        {
            return JsonConvert.SerializeObject(new LogDetails
            {
                Staker = Staker,
                Amount = Amount,
                Reward = Reward
            });
        }
    }
}