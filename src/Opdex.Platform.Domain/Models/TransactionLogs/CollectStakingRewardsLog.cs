using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.TransactionLogs
{
    public class CollectStakingRewardsLog : TransactionLog
    {
        public CollectStakingRewardsLog(dynamic log, string address, int sortOrder)
            : base(TransactionLogType.CollectStakingRewardsLog, address, sortOrder)
        {
            string staker = log?.staker;
            string reward = log?.reward;

            if (!staker.HasValue())
            {
                throw new ArgumentNullException(nameof(staker));
            }
            
            if (!reward.HasValue())
            {
                throw new ArgumentNullException(nameof(reward));
            }

            Staker = staker;
            Reward = reward;
        }
        
        public CollectStakingRewardsLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.CollectStakingRewardsLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Staker = logDetails.Staker;
            Reward = logDetails.Reward;
        }
        
        public string Staker { get; }
        public string Reward { get; }
        
        private struct LogDetails
        {
            public string Staker { get; set; }
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
                Reward = Reward
            });
        }
    }
}