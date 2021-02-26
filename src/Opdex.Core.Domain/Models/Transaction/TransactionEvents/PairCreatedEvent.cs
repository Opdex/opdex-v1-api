using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.Transaction.TransactionEvents
{
    public class PairCreatedEvent : TransactionEvent
    {
        public PairCreatedEvent(dynamic log, string address, int sortOrder) 
            : base(nameof(PairCreatedEvent), address, sortOrder)
        {
            string token = log?.Token;
            string pair = log?.Pair;
            
            if (!token.HasValue())
            {
                throw new ArgumentNullException(nameof(token));
            }
            
            if (!pair.HasValue())
            {
                throw new ArgumentNullException(nameof(pair));
            }

            Token = token;
            Pair = pair;
        }
        
        public string Token { get; }
        public string Pair { get; }
    }
}