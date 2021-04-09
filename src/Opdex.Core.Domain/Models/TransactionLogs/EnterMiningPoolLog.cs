using System;
using Newtonsoft.Json;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionLogs
{
    public class EnterMiningPoolLog : TransactionLog
    {
        public EnterMiningPoolLog(dynamic log, string address, int sortOrder)
            : base(nameof(EnterMiningPoolLog), address, sortOrder)
        {
            string miner = log?.miner;
            string amount = log?.amount;

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
        }
        
        public EnterMiningPoolLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(nameof(EnterMiningPoolLog), id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Miner = logDetails.Miner;
            Amount = logDetails.Amount;
        }

        public string Miner { get; }
        public string Amount { get; }
        
        private sealed class LogDetails
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