using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionEvents
{
    public class TransferEvent : TransactionEvent
    {
        public TransferEvent(dynamic log, string address, int sortOrder) 
            : base(nameof(TransferEvent), address, sortOrder)
        {
            string from = log?.from;
            string to = log?.to;
            string amount = log?.amount;

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
        
        public TransferEvent(long id, long transactionId, string address, int sortOrder, string from, string to, string amount)
            : base(nameof(TransferEvent), id, transactionId, address, sortOrder)
        {
            From = from;
            To = to;
            Amount = amount;
        }
        
        public string From { get; }
        public string To { get; }
        public string Amount { get; }
    }
}