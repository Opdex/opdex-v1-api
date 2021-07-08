using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Domain.Models.TransactionLogs.Governances
{
    public class NominationLog : TransactionLog
    {
        public NominationLog(dynamic log, string address, int sortOrder)
            : base(TransactionLogType.NominationLog, address, sortOrder)
        {
            string stakingPool = log?.stakingPool;
            string miningPool = log?.miningPool;
            string weight = log?.weight;

            if (!stakingPool.HasValue())
            {
                throw new ArgumentNullException(nameof(stakingPool), "Staking pool address must be set.");
            }

            if (!miningPool.HasValue())
            {
                throw new ArgumentNullException(nameof(miningPool), "Mining pool address must be set.");
            }

            if (!weight.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(weight), "Weight must only contain numeric digits.");
            }

            StakingPool = stakingPool;
            MiningPool = miningPool;
            Weight = weight;
        }

        public NominationLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.NominationLog, id, transactionId, address, sortOrder)
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
