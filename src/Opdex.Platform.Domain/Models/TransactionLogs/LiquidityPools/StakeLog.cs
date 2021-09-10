using System;
using Newtonsoft.Json;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools
{
    public abstract class StakeLog : TransactionLog
    {
        protected StakeLog(TransactionLogType logType, Address staker, UInt256 amount, UInt256 totalStaked, UInt256 stakerBalance, Address address, int sortOrder)
            : base(logType, address, sortOrder)
        {
            if (staker == Address.Empty)
            {
                throw new ArgumentNullException(nameof(staker), "Staker address must be set.");
            }

            Staker = staker;
            Amount = amount;
            TotalStaked = totalStaked;
            StakerBalance = stakerBalance;
        }

        protected StakeLog(TransactionLogType logType, long id, long transactionId, Address address, int sortOrder, string details)
            : base(logType, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Staker = logDetails.Staker;
            Amount = logDetails.Amount;
            TotalStaked = logDetails.TotalStaked;
            StakerBalance = logDetails.StakerBalance;
        }

        public Address Staker { get; }
        public UInt256 Amount { get; }
        public UInt256 TotalStaked { get; }
        public UInt256 StakerBalance { get; }

        private struct LogDetails
        {
            public Address Staker { get; set; }
            public UInt256 Amount { get; set; }
            public UInt256 TotalStaked { get; set; }
            public UInt256 StakerBalance { get; set; }
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
