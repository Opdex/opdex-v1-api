using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.TransactionLogs.MiningPools
{
    public class EnableMiningLog : TransactionLog
    {
        public EnableMiningLog(dynamic log, string address, int sortOrder)
            : base(TransactionLogType.EnableMiningLog, address, sortOrder)
        {
            string amount = log?.amount;
            string rewardRate = log?.rewardRate;
            ulong miningPeriodEndBlock = log?.miningPeriodEndBlock;

            if (!amount.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must only contain numeric digits.");
            }

            if (!rewardRate.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(rewardRate), "Reward rate must only contain numeric digits.");
            }

            if (miningPeriodEndBlock < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(miningPeriodEndBlock), "Mining period end block must be greater than 0.");
            }

            Amount = amount;
            RewardRate = rewardRate;
            MiningPeriodEndBlock = miningPeriodEndBlock;
        }

        public EnableMiningLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.EnableMiningLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Amount = logDetails.Amount;
            RewardRate = logDetails.RewardRate;
            MiningPeriodEndBlock = logDetails.MiningPeriodEndBlock;
        }

        public string Amount { get; }
        public string RewardRate { get; }
        public ulong MiningPeriodEndBlock { get; }

        private struct LogDetails
        {
            public string Amount { get; set; }
            public string RewardRate { get; set; }
            public ulong MiningPeriodEndBlock { get; set; }
        }

        private static LogDetails DeserializeLogDetails(string details)
        {
            return JsonConvert.DeserializeObject<LogDetails>(details);
        }

        public override string SerializeLogDetails()
        {
            return JsonConvert.SerializeObject(new LogDetails
            {
                Amount = Amount,
                RewardRate = RewardRate,
                MiningPeriodEndBlock = MiningPeriodEndBlock
            });
        }
    }
}