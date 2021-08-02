using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.TransactionLogs.MiningPools
{
    public abstract class MineLog : TransactionLog
    {
        protected MineLog(TransactionLogType logType, string miner, string amount, string totalSupply, string minerBalance, string address, int sortOrder)
            : base(logType, address, sortOrder)
        {
            if (!miner.HasValue())
            {
                throw new ArgumentNullException(nameof(miner), "Miner address must be set.");
            }

            if (!amount.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must only contain numeric digits.");
            }

            if (!totalSupply.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(totalSupply), "Total supply must only contain numeric digits.");
            }

            if (!minerBalance.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(minerBalance), "Miner balance amount must only contain numeric digits.");
            }

            Miner = miner;
            Amount = amount;
            TotalSupply = totalSupply;
            MinerBalance = minerBalance;
        }

        protected MineLog(TransactionLogType logType, long id, long transactionId, string address, int sortOrder, string details)
            : base(logType, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Miner = logDetails.Miner;
            Amount = logDetails.Amount;
            TotalSupply = logDetails.TotalSupply;
            MinerBalance = logDetails.MinerBalance;
        }

        public string Miner { get; }
        public string Amount { get; }
        public string TotalSupply { get; }
        public string MinerBalance { get; }

        private struct LogDetails
        {
            public string Miner { get; set; }
            public string Amount { get; set; }
            public string TotalSupply { get; set; }
            public string MinerBalance { get; set; }
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
