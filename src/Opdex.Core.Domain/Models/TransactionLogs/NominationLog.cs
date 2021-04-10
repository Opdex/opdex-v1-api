using System;
using Newtonsoft.Json;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionLogs
{
    public class NominationLog : TransactionLog
    {
        public NominationLog(dynamic log, string address, int sortOrder)
            : base(nameof(NominationLog), address, sortOrder)
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
        
        public NominationLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(nameof(NominationLog), id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            StakingPool = logDetails.StakingPool;
            MiningPool = logDetails.MiningPool;
            Weight = logDetails.Weight;
        }
        
        public string StakingPool { get; }
        public string MiningPool { get; }
        public string Weight { get; }
        
        private struct LogDetails
        {
            public string StakingPool { get; set; }
            public string MiningPool { get; set; }
            public string Weight { get; set; }
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
                MiningPool = MiningPool,
                Weight = Weight
            });
        }
    }
}