using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionReceipt.LogEvents
{
    public class TransferEvent : LogEventBase
    {
        public TransferEvent(dynamic log) : base(nameof(TransferEvent))
        {
            string from = log?.From;
            string to = log?.To;
            string amount = log?.Amount;

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
        
        public string From { get; }
        public string To { get; }
        public string Amount { get; }
    }
}