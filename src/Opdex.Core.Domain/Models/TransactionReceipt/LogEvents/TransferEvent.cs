using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionReceipt.LogEvents
{
    public class TransferEvent : LogEventBase
    {
        public TransferEvent(dynamic log) : base(nameof(TransferEvent))
        {
            string from = log?.from;
            string to = log?.to;
            ulong amount = log?.amount;

            if (!from.HasValue())
            {
                throw new ArgumentNullException(nameof(from));
            }
            
            if (!to.HasValue())
            {
                throw new ArgumentNullException(nameof(to));
            }

            if (amount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }

            From = from;
            To = to;
            Amount = amount;
        }
        
        public string From { get; }
        public string To { get; }
        public ulong Amount { get; }
    }
}