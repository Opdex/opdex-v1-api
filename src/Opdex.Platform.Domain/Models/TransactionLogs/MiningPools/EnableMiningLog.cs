using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Domain.Models.TransactionLogs.MiningPools
{
    public class EnableMiningLog : TransactionLog
    {
        public EnableMiningLog(dynamic log, Address address, int sortOrder)
            : base(TransactionLogType.EnableMiningLog, address, sortOrder)
        {
            UInt256 amount = UInt256.Parse((string)log?.amount);
            UInt256 rewardRate = UInt256.Parse((string)log?.rewardRate);
            ulong miningPeriodEndBlock = log?.miningPeriodEndBlock;

            if (miningPeriodEndBlock == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(miningPeriodEndBlock), "Mining period end block must be greater than 0.");
            }

            if (amount == UInt256.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than 0.");
            }

            if (rewardRate == UInt256.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(rewardRate), "Reward rate must be greater than 0.");
            }

            Amount = amount;
            RewardRate = rewardRate;
            MiningPeriodEndBlock = miningPeriodEndBlock;
        }

        public EnableMiningLog(ulong id, ulong transactionId, Address address, int sortOrder, string details)
            : base(TransactionLogType.EnableMiningLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Amount = logDetails.Amount;
            RewardRate = logDetails.RewardRate;
            MiningPeriodEndBlock = logDetails.MiningPeriodEndBlock;
        }

        public UInt256 Amount { get; }
        public UInt256 RewardRate { get; }
        public ulong MiningPeriodEndBlock { get; }

        private struct LogDetails
        {
            public UInt256 Amount { get; set; }
            public UInt256 RewardRate { get; set; }
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
