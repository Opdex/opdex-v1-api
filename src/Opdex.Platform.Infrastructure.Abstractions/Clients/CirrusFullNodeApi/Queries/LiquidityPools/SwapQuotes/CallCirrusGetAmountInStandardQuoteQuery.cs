using MediatR;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.SwapQuotes
{
    public class CallCirrusGetAmountInStandardQuoteQuery : IRequest<string>
    {
        public CallCirrusGetAmountInStandardQuoteQuery(string market, string amountOut, string  reserveIn, string reserveOut)
        {
            if (!market.HasValue())
            {
                throw new ArgumentNullException(nameof(market));
            }

            if (!amountOut.HasValue())
            {
                throw new ArgumentNullException(nameof(amountOut));
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
            AmountOut = amountOut;
            ReserveIn = reserveIn;
            ReserveOut = reserveOut;
        }

        public string Market { get; }
        public string AmountOut { get; }
        public string ReserveIn { get; }
        public string ReserveOut { get; }
    }
}
