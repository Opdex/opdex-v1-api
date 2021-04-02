using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionEvents
{
    public class PoolCreatedEvent : TransactionEvent
    {
        public PoolCreatedEvent(dynamic log, string address, int sortOrder) 
            : base(nameof(PoolCreatedEvent), address, sortOrder)
        {
            string token = log?.token;
            string pool = log?.pool;
            
            if (!token.HasValue())
            {
                throw new ArgumentNullException(nameof(token));
            }
            
            if (!pool.HasValue())
            {
                throw new ArgumentNullException(nameof(pool));
            }

            Token = token;
            Pool = pool;
        }
        
        public PoolCreatedEvent(long id, long transactionId, string address, int sortOrder, string token, string pool)
            : base(nameof(PoolCreatedEvent), id, transactionId, address, sortOrder)
        {
            Token = token;
            Pool = pool;
        }
        
        public string Token { get; }
        public string Pool { get; }
    }
}