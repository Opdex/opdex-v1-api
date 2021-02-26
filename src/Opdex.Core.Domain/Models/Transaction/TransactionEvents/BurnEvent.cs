using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.Transaction.TransactionEvents
{
    public class BurnEvent : TransactionEvent
    {
        public BurnEvent(dynamic log, string address, int sortOrder) 
            : base(nameof(BurnEvent), address, sortOrder)
        {
            string sender = log?.Sender;
            string to = log?.To;
            ulong amountCrs = log?.AmountCrs;
            string amountSrc = log?.AmountSrc;

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
                throw new ArgumentNullException(nameof(amountSrc));
            }

            Sender = sender;
            To = to;
            AmountCrs = amountCrs;
            AmountSrc = amountSrc;
        }
        
        public string Sender { get; }
        public string To { get; }
        public ulong AmountCrs { get; }
        public string AmountSrc { get; }
    }
}