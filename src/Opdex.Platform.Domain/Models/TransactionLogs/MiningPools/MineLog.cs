using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Domain.Models.TransactionLogs.MiningPools
{
    public abstract class MineLog : TransactionLog
    {
        protected MineLog(TransactionLogType logType, Address miner, UInt256 amount, UInt256 totalSupply, UInt256 minerBalance, Address address, int sortOrder)
            : base(logType, address, sortOrder)
        {
            if (miner == Address.Empty)
            {
                throw new ArgumentNullException(nameof(miner), "Miner address must be set.");
            }

            if (amount == UInt256.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than 0.");
            }

            Miner = miner;
            Amount = amount;
            TotalSupply = totalSupply;
            MinerBalance = minerBalance;
        }

        protected MineLog(TransactionLogType logType, ulong id, ulong transactionId, Address address, int sortOrder, string details)
            : base(logType, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Miner = logDetails.Miner;
            Amount = logDetails.Amount;
            TotalSupply = logDetails.TotalSupply;
            MinerBalance = logDetails.MinerBalance;
        }

        public Address Miner { get; }
        public UInt256 Amount { get; }
        public UInt256 TotalSupply { get; }
        public UInt256 MinerBalance { get; }

        private struct LogDetails
        {
            public Address Miner { get; set; }
            public UInt256 Amount { get; set; }
            public UInt256 TotalSupply { get; set; }
            public UInt256 MinerBalance { get; set; }
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
                Amount = Amount,
                TotalSupply = TotalSupply,
                MinerBalance = MinerBalance
            });
        }
    }
}
