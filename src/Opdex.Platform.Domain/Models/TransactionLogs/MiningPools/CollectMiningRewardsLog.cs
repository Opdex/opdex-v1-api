using System;
using Newtonsoft.Json;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Domain.Models.TransactionLogs.MiningPools
{
    public class CollectMiningRewardsLog : TransactionLog
    {
        public CollectMiningRewardsLog(dynamic log, Address address, int sortOrder)
            : base(TransactionLogType.CollectMiningRewardsLog, address, sortOrder)
        {
            Address miner = log?.miner;
            UInt256 amount = UInt256.Parse(log?.amount);

            if (miner == Address.Empty)
            {
                throw new ArgumentNullException(nameof(miner), "Miner address must be set.");
            }

            Miner = miner;
            Amount = amount;
        }

        public CollectMiningRewardsLog(long id, long transactionId, Address address, int sortOrder, string details)
            : base(TransactionLogType.CollectMiningRewardsLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Miner = logDetails.Miner;
            Amount = logDetails.Amount;
        }

        public Address Miner { get; }
        public UInt256 Amount { get; }

        private struct LogDetails
        {
            public Address Miner { get; set; }
            public UInt256 Amount { get; set; }
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
