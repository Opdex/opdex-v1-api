using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools
{
    public class BurnLog : TransactionLog
    {
        public BurnLog(dynamic log, string address, int sortOrder) 
            : base(TransactionLogType.BurnLog, address, sortOrder)
        {
            string sender = log?.sender;
            string to = log?.to;
            ulong amountCrs = log?.amountCrs;
            string amountSrc = log?.amountSrc;
            string amountLpt = log?.amountLpt;

            if (!sender.HasValue())
            {
                throw new ArgumentNullException(nameof(sender));
            }
            
            if (!to.HasValue())
            {
                throw new ArgumentNullException(nameof(to));
            }
            
            if (amountCrs < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(amountCrs));
            }
            
            if (!amountSrc.IsNumeric())
            {
                throw new ArgumentNullException(nameof(amountSrc));
            }
            
            if (!amountLpt.IsNumeric())
            {
                throw new ArgumentNullException(nameof(amountLpt));
            }

            Sender = sender;
            To = to;
            AmountCrs = amountCrs;
            AmountSrc = amountSrc;
            AmountLpt = amountLpt;
        }
        
        public BurnLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.BurnLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Sender = logDetails.Sender;
            To = logDetails.To;
            AmountCrs = logDetails.AmountCrs;
            AmountSrc = logDetails.AmountSrc;
            AmountLpt = logDetails.AmountLpt;
        }
        
        public string Sender { get; }
        public string To { get; }
        public ulong AmountCrs { get; }
        public string AmountSrc { get; }
        public string AmountLpt { get; }
        
        private struct LogDetails
        {
            public string Sender { get; set; }
            public string To { get; set; }
            public ulong AmountCrs { get; set; }
            public string AmountSrc { get; set; }
            public string AmountLpt { get; set; }
        }

        private static LogDetails DeserializeLogDetails(string details)
        {
            return JsonConvert.DeserializeObject<LogDetails>(details);
        }

        public override string SerializeLogDetails()
        {
            return JsonConvert.SerializeObject(new LogDetails
            {
                Sender = Sender,
                To = To,
                AmountCrs = AmountCrs,
                AmountSrc = AmountSrc,
                AmountLpt = AmountLpt
            });
        }
    }
}