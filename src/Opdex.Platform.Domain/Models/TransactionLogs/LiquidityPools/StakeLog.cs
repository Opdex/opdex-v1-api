using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools
{
    public abstract class StakeLog : TransactionLog
    {
        protected StakeLog(TransactionLogType logType, string staker, string amount, string totalStaked, string stakerBalance, string address, int sortOrder)
            : base(logType, address, sortOrder)
        {
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

            if (!stakerBalance.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(stakerBalance), "Staker balance must only contain numeric digits.");
            }

            Staker = staker;
            Amount = amount;
            TotalStaked = totalStaked;
            StakerBalance = stakerBalance;
        }

        protected StakeLog(TransactionLogType logType, long id, long transactionId, string address, int sortOrder, string details)
            : base(logType, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Staker = logDetails.Staker;
            Amount = logDetails.Amount;
            TotalStaked = logDetails.TotalStaked;
            StakerBalance = logDetails.StakerBalance;
        }

        public string Staker { get; }
        public string Amount { get; }
        public string TotalStaked { get; }
        public string StakerBalance { get; }

        private struct LogDetails
        {
            public string Staker { get; set; }
            public string Amount { get; set; }
            public string TotalStaked { get; set; }
            public string StakerBalance { get; set; }
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
                TotalStaked = TotalStaked,
                StakerBalance = StakerBalance
            });
        }
    }
}
