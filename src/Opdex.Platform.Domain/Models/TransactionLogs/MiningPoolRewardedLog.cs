using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.TransactionLogs
{
    public class MiningPoolRewardedLog : TransactionLog
    {
        public MiningPoolRewardedLog(dynamic log, string address, int sortOrder)
            : base(nameof(MiningPoolRewardedLog), address, sortOrder)
        {
            string amount = log?.amount;

            if (!amount.HasValue())
            {
                throw new ArgumentNullException(nameof(amount));
            }

            Amount = amount;
        }
        
        public MiningPoolRewardedLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(nameof(MiningPoolRewardedLog), id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Amount = logDetails.Amount;
        }
        
        public string Amount { get; }
        
        private struct LogDetails
        {
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
                Amount = Amount
            });
        }
    }
}