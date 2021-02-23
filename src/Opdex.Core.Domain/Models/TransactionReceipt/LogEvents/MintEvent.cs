using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionReceipt.LogEvents
{
    public class MintEvent : LogEventBase
    {
        public MintEvent(dynamic log) : base(nameof(MintEvent))
        {
            string sender = log?.sender;
            ulong amountCrs = log?.amountCrs;
            ulong amountSrc = log?.amountSrc;

            if (!sender.HasValue())
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (amountCrs < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(amountCrs));
            }
            
            if (amountSrc < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(amountCrs));
            }

            Sender = sender;
            AmountCrs = amountCrs;
            AmountSrc = amountSrc;
        }
        
        public string Sender { get; }
        public ulong AmountCrs { get; }
        public ulong AmountSrc { get; }
    }
}