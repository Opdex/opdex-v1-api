using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionLogs
{
    public class LiquidityPoolCreatedLog : TransactionLog
    {
        public LiquidityPoolCreatedLog(dynamic log, string address, int sortOrder) 
            : base(nameof(LiquidityPoolCreatedLog), address, sortOrder)
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
        
        public LiquidityPoolCreatedLog(long id, long transactionId, string address, int sortOrder, string token, string pool)
            : base(nameof(LiquidityPoolCreatedLog), id, transactionId, address, sortOrder)
        {
            Token = token;
            Pool = pool;
        }
        
        public string Token { get; }
        public string Pool { get; }
    }
}