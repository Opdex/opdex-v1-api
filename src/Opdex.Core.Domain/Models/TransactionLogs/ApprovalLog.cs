using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionLogs
{
    public class ApprovalLog : TransactionLog
    {
        public ApprovalLog(dynamic log, string address, int sortOrder) 
            : base(nameof(ApprovalLog), address, sortOrder)
        {
            string owner = log?.owner;
            string spender = log?.spender;
            string amount = log?.amount;
            
            if (!owner.HasValue())
            {
                throw new ArgumentNullException(nameof(owner));
            }
            
            if (!spender.HasValue())
            {
                throw new ArgumentNullException(nameof(spender));
            }
            
            if (!amount.HasValue())
            {
                throw new ArgumentNullException(nameof(amount));
            }

            Owner = owner;
            Spender = spender;
            Amount = amount;
        }

        public ApprovalLog(long id, long transactionId, string address, int sortOrder, string owner, string spender, string amount)
            : base(nameof(ApprovalLog), id, transactionId, address, sortOrder)
        {
            Owner = owner;
            Spender = spender;
            Amount = amount;
        }
        
        public string Owner { get; }
        public string Spender { get; }
        public string Amount { get; }
    }
}