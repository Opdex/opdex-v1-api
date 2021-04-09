using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionLogs
{
    public class TransferLog : TransactionLog
    {
        public TransferLog(dynamic log, string address, int sortOrder) 
            : base(nameof(TransferLog), address, sortOrder)
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
        
        public TransferLog(long id, long transactionId, string address, int sortOrder, string from, string to, string amount)
            : base(nameof(TransferLog), id, transactionId, address, sortOrder)
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