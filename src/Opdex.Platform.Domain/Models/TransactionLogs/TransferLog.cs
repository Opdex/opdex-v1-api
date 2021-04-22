using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.TransactionLogs
{
    public class TransferLog : TransactionLog
    {
        public TransferLog(dynamic log, string address, int sortOrder) 
            : base(nameof(TransferLog), address, sortOrder)
        {
            string from = log?.from;
            string to = log?.to;
            string amount = log?.amount;

            if (!from.HasValue())
            {
                throw new ArgumentNullException(nameof(from));
            }
            
            if (!to.HasValue())
            {
                throw new ArgumentNullException(nameof(to));
            }

            if (!amount.HasValue())
            {
                throw new ArgumentNullException(nameof(amount));
            }

            From = from;
            To = to;
            Amount = amount;
        }
        
        public TransferLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(nameof(TransferLog), id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            From = logDetails.From;
            To = logDetails.To;
            Amount = logDetails.Amount;
        }
        
        public string From { get; }
        public string To { get; }
        public string Amount { get; }
        
        private struct LogDetails
        {
            public string From { get; set; }
            public string To { get; set; }
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
                From = From,
                To = To,
                Amount = Amount
            });
        }
    }
}