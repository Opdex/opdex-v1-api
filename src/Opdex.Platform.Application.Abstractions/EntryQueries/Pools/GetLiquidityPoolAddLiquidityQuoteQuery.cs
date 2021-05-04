using System;
using MediatR;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Pools
{
    public class GetLiquidityPoolAddLiquidityQuoteQuery : IRequest<string>
    {
        public GetLiquidityPoolAddLiquidityQuoteQuery(string amountCrsIn, string amountSrcIn, string pool, string market)
        {
            if (!pool.HasValue())
            {
                throw new ArgumentNullException(nameof(pool));
            }
            
            if (!market.HasValue())
            {
                throw new ArgumentNullException(nameof(market));
            }
            
            AmountCrsIn = amountCrsIn;
            AmountSrcIn = amountSrcIn;
            Pool = pool;
            Market = market;
        }
        
        public string AmountCrsIn { get; }
        public string AmountSrcIn { get; }
        public string Pool { get; }
        public string Market { get; }
    }
}