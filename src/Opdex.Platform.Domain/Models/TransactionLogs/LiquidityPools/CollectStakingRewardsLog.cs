using System;
using Newtonsoft.Json;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools
{
    public class CollectStakingRewardsLog : TransactionLog
    {
        public CollectStakingRewardsLog(dynamic log, Address address, int sortOrder)
            : base(TransactionLogType.CollectStakingRewardsLog, address, sortOrder)
        {
            Address staker = log?.staker;
            UInt256 amount = UInt256.Parse(log?.amount);

            if (staker == Address.Empty)
            {
                throw new ArgumentNullException(nameof(staker), "Staker address must be set.");
            }

            Staker = staker;
            Amount = amount;
        }

        public CollectStakingRewardsLog(long id, long transactionId, Address address, int sortOrder, string details)
            : base(TransactionLogType.CollectStakingRewardsLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Staker = logDetails.Staker;
            Amount = logDetails.Amount;
        }

        public Address Staker { get; }
        public UInt256 Amount { get; }

        private struct LogDetails
        {
            public Address Staker { get; set; }
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
                Staker = Staker,
                Amount = Amount
            });
        }
    }
}
