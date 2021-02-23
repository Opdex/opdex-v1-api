using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionReceipt.LogEvents
{
    public class SwapEvent : LogEventBase
    {
        public SwapEvent(dynamic log) : base(nameof(SwapEvent))
        {
            string sender = log?.sender;
            string to = log?.to;
            ulong amountCrsIn = log?.amountCrsIn;
            ulong amountCrsOut = log?.amountCrsOut;
            ulong amountSrcIn = log?.amountSrcIn;
            ulong amountSrcOut = log?.amoungTokenOut;

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
            
            if (amountSrcIn < 1 && amountSrcOut < 1)
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
        public ulong AmountSrcIn { get; }
        public ulong AmountCrsOut { get; }
        public ulong AmountSrcOut { get; }
    }
}