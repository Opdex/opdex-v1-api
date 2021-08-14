using MediatR;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.SwapQuotes
{
    public class CallCirrusGetAmountOutMultiHopQuoteQuery : IRequest<string>
    {
        public CallCirrusGetAmountOutMultiHopQuoteQuery(string market, string tokenInAmount, string  tokenInReserveCrs, string tokenInReserveSrc,
            string tokenOutReserveCrs, string tokenOutReserveSrc)
        {
            if (!market.HasValue())
            {
                throw new ArgumentNullException(nameof(market));
            }

            if (!tokenInAmount.HasValue())
            {
                throw new ArgumentNullException(nameof(tokenInAmount));
            }

            if (!tokenInReserveCrs.HasValue())
            {
                throw new ArgumentNullException(nameof(tokenInReserveCrs));
            }

            if (!tokenInReserveSrc.HasValue())
            {
                throw new ArgumentNullException(nameof(tokenInReserveSrc));
            }

            if (!tokenOutReserveCrs.HasValue())
            {
                throw new ArgumentNullException(nameof(tokenOutReserveCrs));
            }

            if (!tokenOutReserveSrc.HasValue())
            {
                throw new ArgumentNullException(nameof(tokenOutReserveSrc));
            }

            Market = market;
            TokenInAmount = tokenInAmount;
            TokenInReserveCrs = tokenInReserveCrs;
            TokenInReserveSrc = tokenInReserveSrc;
            TokenOutReserveCrs = tokenOutReserveCrs;
            TokenOutReserveSrc = tokenOutReserveSrc;
        }

        public string Market { get; }
        public string TokenInAmount { get; }
        public string TokenOutReserveCrs { get; }
        public string TokenOutReserveSrc { get; }
        public string TokenInReserveCrs { get; }
        public string TokenInReserveSrc { get; }
    }
}
