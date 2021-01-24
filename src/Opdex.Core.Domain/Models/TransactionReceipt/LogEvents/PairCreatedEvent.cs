using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionReceipt.LogEvents
{
    public class PairCreatedEvent : LogEventBase
    {
        public PairCreatedEvent(dynamic log) : base(nameof(PairCreatedEvent))
        {
            string token = log?.token;
            string pair = log?.pair;
            
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