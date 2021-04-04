using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionEvents
{
    public class OwnerChangeEvent : TransactionEvent
    {
        public OwnerChangeEvent(dynamic log, string address, int sortOrder)
            : base(nameof(StakeEvent), address, sortOrder)
        {
            string from = log?.from;
            string to = log?.to;

            if (!from.HasValue())
            {
                throw new ArgumentNullException(nameof(from));
            }
            
            if (!to.HasValue())
            {
                throw new ArgumentNullException(nameof(to));
            }

            From = from;
            To = to;
        }
        
        public string From { get; }
        public string To { get; }
    }
}