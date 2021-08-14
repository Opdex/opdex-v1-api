using MediatR;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.LiquidityQuotes
{
    public class CallCirrusGetAddLiquidityQuoteQuery : IRequest<string>
    {
        public CallCirrusGetAddLiquidityQuoteQuery(string amountA, string reserveA, string reserveB, string market)
        {
            if (!amountA.HasValue())
            {
                throw new ArgumentNullException(nameof(amountA));
            }

            if (!reserveA.HasValue())
            {
                throw new ArgumentNullException(nameof(reserveA));
            }

            if (!reserveB.HasValue())
            {
                throw new ArgumentNullException(nameof(reserveB));
            }

            if (!market.HasValue())
            {
                throw new ArgumentNullException(nameof(market));
            }

            AmountA = amountA;
            ReserveA = reserveA;
            ReserveB = reserveB;
            Market = market;
        }

        public string AmountA { get; }
        public string ReserveA { get; }
        public string ReserveB { get; }
        public string Market { get; }
    }
}
