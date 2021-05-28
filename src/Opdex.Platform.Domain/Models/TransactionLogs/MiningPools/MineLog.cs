using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.TransactionLogs.MiningPools
{
    public class MineLog : TransactionLog
    {
        public MineLog(dynamic log, string address, int sortOrder)
            : base(TransactionLogType.MineLog, address, sortOrder)
        {
            string miner = log?.miner;
            string amount = log?.amount;
            string totalSupply = log?.totalSupply;
            byte eventType = log?.eventType;

            if (!miner.HasValue())
            {
                throw new ArgumentNullException(nameof(miner));
            }
            
            if (!amount.HasValue())
            {
                throw new ArgumentNullException(nameof(amount));
            }

            Miner = miner;
            Amount = amount;
            TotalSupply = totalSupply;
            EventType = eventType;
        }
        
        public MineLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.MineLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Miner = logDetails.Miner;
            Amount = logDetails.Amount;
            TotalSupply = logDetails.TotalSupply;
            EventType = logDetails.EventType;
        }

        public string Miner { get; }
        public string Amount { get; }
        public string TotalSupply { get; }
        public byte EventType { get; }
        
        private struct LogDetails
        {
            public string Miner { get; set; }
            public string Amount { get; set; }
            public string TotalSupply { get; set; }
            public byte EventType { get; set; }
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