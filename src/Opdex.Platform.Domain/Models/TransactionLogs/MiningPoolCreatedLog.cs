using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.TransactionLogs
{
    public class MiningPoolCreatedLog : TransactionLog
    {
        public MiningPoolCreatedLog(dynamic log, string address, int sortOrder)
            : base(nameof(MiningPoolCreatedLog), address, sortOrder)
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
        
        public MiningPoolCreatedLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(nameof(MiningPoolCreatedLog), id, transactionId, address, sortOrder)
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