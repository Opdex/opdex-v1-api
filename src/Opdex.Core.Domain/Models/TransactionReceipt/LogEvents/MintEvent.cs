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
            ulong amountToken = log?.amountToken;

            if (!sender.HasValue())
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (amountCrs < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(amountCrs));
            }
            
            if (amountToken < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(amountCrs));
            }

            Sender = sender;
            AmountCrs = amountCrs;
            AmountToken = amountToken;
        }
        
        public string Sender { get; }
        public ulong AmountCrs { get; }
        public ulong AmountToken { get; }
    }
}