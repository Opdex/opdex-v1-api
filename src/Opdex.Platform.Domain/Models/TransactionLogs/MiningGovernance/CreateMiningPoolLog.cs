using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.TransactionLogs.MiningGovernance
{
    public class CreateMiningPoolLog : TransactionLog
    {
        public CreateMiningPoolLog(dynamic log, string address, int sortOrder)
            : base(TransactionLogType.CreateMiningPoolLog, address, sortOrder)
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
        
        public CreateMiningPoolLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.CreateMiningPoolLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            StakingPool = logDetails.StakingPool;
            MiningPool = logDetails.MiningPool;
        }
        
        public string StakingPool { get; }
        public string MiningPool { get; }
        
        private struct LogDetails
        {
            public string StakingPool { get; set; }
            public string MiningPool { get; set; }
        }

        private static LogDetails DeserializeLogDetails(string details)
        {
            return JsonConvert.DeserializeObject<LogDetails>(details);
        }

        public override string SerializeLogDetails()
        {
            return JsonConvert.SerializeObject(new LogDetails
            {
                StakingPool = StakingPool,
                MiningPool = MiningPool
            });
        }
    }
}