using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionEvents
{
    public class SwapEvent : TransactionEvent
    {
        public SwapEvent(dynamic log, string address, int sortOrder) 
            : base(nameof(SwapEvent), address, sortOrder)
        {
            string sender = log?.sender;
            string to = log?.to;
            ulong amountCrsIn = log?.amountCrsIn;
            ulong amountCrsOut = log?.amountCrsOut;
            string amountSrcIn = log?.amountSrcIn;
            string amountSrcOut = log?.amountSrcOut;

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
        
        public SwapEvent(long id, long transactionId, string address, int sortOrder, string sender, string to, 
            ulong amountCrsIn, ulong amountCrsOut, string amountSrcIn, string amountSrcOut)
            : base(nameof(SwapEvent), id, transactionId, address, sortOrder)
        {
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