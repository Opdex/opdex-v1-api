using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionReceipt.LogEvents
{
    public class ApprovalEvent : LogEventBase
    {
        public ApprovalEvent(dynamic log) : base(nameof(ApprovalEvent))
        {
            string owner = log?.Owner;
            string spender = log?.Spender;
            string amount = log?.Amount;
            
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
        
        public string Owner { get; }
        public string Spender { get; }
        public string Amount { get; }
    }
}