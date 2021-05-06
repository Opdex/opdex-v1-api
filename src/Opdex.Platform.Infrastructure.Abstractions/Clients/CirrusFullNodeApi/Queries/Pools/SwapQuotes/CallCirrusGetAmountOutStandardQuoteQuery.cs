using System;
using MediatR;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Pools.SwapQuotes
{
    public class CallCirrusGetAmountOutStandardQuoteQuery : IRequest<string>
    {
        public CallCirrusGetAmountOutStandardQuoteQuery(string market, string amountIn, string  reserveIn, string reserveOut)
        {
            if (!market.HasValue())
            {
                throw new ArgumentNullException(nameof(market));
            }
            
            if (!amountIn.HasValue())
            {
                throw new ArgumentNullException(nameof(amountIn));
            }
            
            if (!reserveIn.HasValue())
            {
                throw new ArgumentNullException(nameof(reserveIn));
            }
            
            if (!reserveOut.HasValue())
            {
                throw new ArgumentNullException(nameof(reserveOut));
            }

            Market = market;
            AmountIn = amountIn;
            ReserveIn = reserveIn;
            ReserveOut = reserveOut;
        }
        
        public string Market { get; }
        public string AmountIn { get; }
        public string ReserveIn { get; }
        public string ReserveOut { get; }
    }
}