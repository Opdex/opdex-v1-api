using System;
using Newtonsoft.Json;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionLogs
{
    public class MintLog : TransactionLog
    {
        public MintLog(dynamic log, string address, int sortOrder) 
            : base(nameof(MintLog), address, sortOrder)
        {
            string sender = log?.sender;
            string to = log?.to;
            ulong amountCrs = log?.amountCrs;
            string amountSrc = log?.amountSrc;

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
            
            if (!amountSrc.HasValue())
            {
                throw new ArgumentNullException(nameof(amountCrs));
            }

            Sender = sender;
            To = to;
            AmountCrs = amountCrs;
            AmountSrc = amountSrc;
        }
        
        public MintLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(nameof(MintLog), id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Sender = logDetails.Sender;
            To = logDetails.To;
            AmountCrs = logDetails.AmountCrs;
            AmountSrc = logDetails.AmountSrc;
        }
        
        public string Sender { get; }
        public string To { get; }
        public ulong AmountCrs { get; }
        public string AmountSrc { get; }
        
        private sealed class LogDetails
        {
            public string Sender { get; set; }
            public string To { get; set; }
            public ulong AmountCrs { get; set; }
            public string AmountSrc { get; set; }
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
                AmountSrc = AmountSrc
            });
        }
    }
}