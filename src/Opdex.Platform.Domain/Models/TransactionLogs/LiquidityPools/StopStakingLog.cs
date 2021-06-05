using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools
{
    public class StopStakingLog : TransactionLog
    {
        public StopStakingLog(dynamic log, string address, int sortOrder)
            : base(TransactionLogType.StopStakingLog, address, sortOrder)
        {
            string staker = log?.staker;
            string amount = log?.amount;
            string totalStaked = log?.totalStaked;

            if (!staker.HasValue())
            {
                throw new ArgumentNullException(nameof(staker), "Staker address must be set.");
            }

            if (!amount.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must only contain numeric digits.");
            }

            if (!totalStaked.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(totalStaked), "Total staked amount must only contain numeric digits.");
            }

            Staker = staker;
            Amount = amount;
            TotalStaked = totalStaked;
        }

        public StopStakingLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.StopStakingLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Staker = logDetails.Staker;
            Amount = logDetails.Amount;
            TotalStaked = logDetails.TotalStaked;
        }

        public string Staker { get; }
        public string Amount { get; }
        public string TotalStaked { get; }

        private struct LogDetails
        {
            public string Staker { get; set; }
            public string Amount { get; set; }
            public string TotalStaked { get; set; }
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
                Amount = Amount,
                TotalStaked = TotalStaked
            });
        }
    }
}