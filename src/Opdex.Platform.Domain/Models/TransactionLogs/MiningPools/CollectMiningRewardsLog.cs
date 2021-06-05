using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.TransactionLogs.MiningPools
{
    public class CollectMiningRewardsLog : TransactionLog
    {
        public CollectMiningRewardsLog(dynamic log, string address, int sortOrder)
            : base(TransactionLogType.CollectMiningRewardsLog, address, sortOrder)
        {
            string miner = log?.miner;
            string amount = log?.amount;

            if (!miner.HasValue())
            {
                throw new ArgumentNullException(nameof(miner), "Miner address must be set.");
            }

            if (!amount.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must only contain numeric digits.");
            }

            Miner = miner;
            Amount = amount;
        }

        public CollectMiningRewardsLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.CollectMiningRewardsLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Miner = logDetails.Miner;
            Amount = logDetails.Amount;
        }

        public string Miner { get; }
        public string Amount { get; }

        private struct LogDetails
        {
            public string Miner { get; set; }
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
                Miner = Miner,
                Amount = Amount
            });
        }
    }
}