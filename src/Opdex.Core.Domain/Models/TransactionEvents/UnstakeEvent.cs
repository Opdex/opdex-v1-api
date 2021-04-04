using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionEvents
{
    public class UnstakeEvent : TransactionEvent
    {
        public UnstakeEvent(dynamic log, string address, int sortOrder)
            : base(nameof(StakeEvent), address, sortOrder)
        {
            string staker = log?.staker;
            string amount = log?.amount;

            if (!staker.HasValue())
            {
                throw new ArgumentNullException(nameof(staker));
            }
            
            if (!amount.HasValue())
            {
                throw new ArgumentNullException(nameof(amount));
            }

            Staker = staker;
            Amount = amount;
        }
        
        public string Staker { get; }
        public string Amount { get; }
    }
}