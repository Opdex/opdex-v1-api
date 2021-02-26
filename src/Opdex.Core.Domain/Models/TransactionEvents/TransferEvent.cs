using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionEvents
{
    public class TransferEvent : TransactionEvent
    {
        public TransferEvent(dynamic log, string address, int sortOrder) 
            : base(nameof(TransferEvent), address, sortOrder)
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