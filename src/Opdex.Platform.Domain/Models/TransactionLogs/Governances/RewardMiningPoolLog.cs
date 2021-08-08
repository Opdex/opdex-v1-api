using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Domain.Models.TransactionLogs.Governances
{
    public class RewardMiningPoolLog : TransactionLog
    {
        public RewardMiningPoolLog(dynamic log, string address, int sortOrder)
            : base(TransactionLogType.RewardMiningPoolLog, address, sortOrder)
        {
            string stakingPool = log?.stakingPool;
            string miningPool = log?.miningPool;
            string amount = log?.amount;

            if (!stakingPool.HasValue())
            {
                throw new ArgumentNullException(nameof(stakingPool), "Staking pool address must be set.");
            }

            if (!miningPool.HasValue())
            {
                throw new ArgumentNullException(nameof(miningPool), "Mining pool address must be set.");
            }

            if (!amount.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must only contain numeric digits.");
            }

            StakingPool = stakingPool;
            MiningPool = miningPool;
            Amount = amount;
        }

        public RewardMiningPoolLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.RewardMiningPoolLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            StakingPool = logDetails.StakingPool;
            MiningPool = logDetails.MiningPool;
            Amount = logDetails.Amount;
        }

        public string StakingPool { get; }
        public string MiningPool { get; }
        public string Amount { get; }

        private struct LogDetails
        {
            public string StakingPool { get; set; }
            public string MiningPool { get; set; }
            public string Amount { get; set; }
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
                Amount = Amount
            });
        }
    }
}
