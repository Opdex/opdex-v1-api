using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionEvents
{
    public class MintEvent : TransactionEvent
    {
        public MintEvent(dynamic log, string address, int sortOrder) 
            : base(nameof(MintEvent), address, sortOrder)
        {
            string sender = log?.sender;
            ulong amountCrs = log?.amountCrs;
            string amountSrc = log?.amountSrc;

            if (!sender.HasValue())
            {
                throw new ArgumentNullException(nameof(sender));
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
            AmountCrs = amountCrs;
            AmountSrc = amountSrc;
        }
        
        public MintEvent(long id, long transactionId, string address, int sortOrder, string sender, ulong amountCrs, string amountSrc)
            : base(nameof(MintEvent), id, transactionId, address, sortOrder)
        {
            Sender = sender;
            AmountCrs = amountCrs;
            AmountSrc = amountSrc;
        }
        
        public string Sender { get; }
        public ulong AmountCrs { get; }
        public string AmountSrc { get; }
    }
}