using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.Transaction.TransactionEvents
{
    public class SwapEvent : TransactionEvent
    {
        public SwapEvent(dynamic log, string address, int sortOrder) 
            : base(nameof(SwapEvent), address, sortOrder)
        {
            string sender = log?.Sender;
            string to = log?.To;
            ulong amountCrsIn = log?.AmountCrsIn;
            ulong amountCrsOut = log?.AmountCrsOut;
            string amountSrcIn = log?.AmountSrcIn;
            string amountSrcOut = log?.AmountSrcOut;

            if (!sender.HasValue())
            {
                throw new ArgumentNullException(nameof(sender));
            }
            
            if (!to.HasValue())
            {
                throw new ArgumentNullException(nameof(to));
            }

            if (amountCrsIn < 1 && amountCrsOut < 1)
            {
                throw new Exception($"{nameof(amountCrsIn)} or {nameof(amountCrsOut)} must be greater than 0.");
            }
            
            if (!amountSrcIn.HasValue() && !amountSrcOut.HasValue())
            {
                throw new Exception($"{nameof(amountSrcIn)} or {nameof(amountSrcOut)} must be greater than 0.");
            }

            Sender = sender;
            To = to;
            AmountCrsIn = amountCrsIn;
            AmountCrsOut = amountCrsOut;
            AmountSrcIn = amountSrcIn;
            AmountSrcOut = amountSrcOut;
        }
        
        public string Sender { get; }
        public string To { get; }
        public ulong AmountCrsIn { get; }
        public string AmountSrcIn { get; }
        public ulong AmountCrsOut { get; }
        public string AmountSrcOut { get; }
    }
}