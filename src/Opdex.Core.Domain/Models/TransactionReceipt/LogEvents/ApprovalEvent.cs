using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionReceipt.LogEvents
{
    public class ApprovalEvent : LogEventBase
    {
        public ApprovalEvent(dynamic log) : base(nameof(ApprovalEvent))
        {
            string owner = log?.owner;
            string spender = log?.spender;
            ulong amount = log?.amount;
            
            if (!owner.HasValue())
            {
                throw new ArgumentNullException(nameof(owner));
            }
            
            if (!spender.HasValue())
            {
                throw new ArgumentNullException(nameof(spender));
            }

            Owner = owner;
            Spender = spender;
            Amount = amount;
        }
        
        public string Owner { get; }
        public string Spender { get; }
        public ulong Amount { get; }
    }
}