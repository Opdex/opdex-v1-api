using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools
{
    public class CollectStakingRewardsLog : TransactionLog
    {
        public CollectStakingRewardsLog(dynamic log, string address, int sortOrder)
            : base(TransactionLogType.CollectStakingRewardsLog, address, sortOrder)
        {
            string staker = log?.staker;
            string amount = log?.amount;

            if (!staker.HasValue())
            {
                throw new ArgumentNullException(nameof(staker), "Staker address must be set.");
            }

            if (!amount.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Reward must only contain numeric digits.");
            }

            Staker = staker;
            Amount = amount;
        }

        public CollectStakingRewardsLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.CollectStakingRewardsLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Staker = logDetails.Staker;
            Amount = logDetails.Amount;
        }

        public string Staker { get; }
        public string Amount { get; }

        private struct LogDetails
        {
            public string Staker { get; set; }
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
                Staker = Staker,
                Amount = Amount
            });
        }
    }
}
