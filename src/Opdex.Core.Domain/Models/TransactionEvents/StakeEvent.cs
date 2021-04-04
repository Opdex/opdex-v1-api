using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionEvents
{
    public class StakeEvent : TransactionEvent
    {
        public StakeEvent(dynamic log, string address, int sortOrder)
            : base(nameof(StakeEvent), address, sortOrder)
        {
            string staker = log?.staker;
            string amount = log?.amount;
            string weight = log?.weight;

            if (!staker.HasValue())
            {
                throw new ArgumentNullException(nameof(staker));
            }
            
            if (!amount.HasValue())
            {
                throw new ArgumentNullException(nameof(amount));
            }
            
            if (!weight.HasValue())
            {
                throw new ArgumentNullException(nameof(weight));
            }

            Staker = staker;
            Amount = amount;
            Weight = weight;
        }
        
        public string Staker { get; }
        public string Amount { get; }
        public string Weight { get; }
    }
}