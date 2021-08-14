using MediatR;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.SwapQuotes
{
    public class CallCirrusGetAmountInMultiHopQuoteQuery : IRequest<string>
    {
        public CallCirrusGetAmountInMultiHopQuoteQuery(string market, string tokenOutAmount, string  tokenOutReserveCrs, string tokenOutReserveSrc,
            string tokenInReserveCrs, string tokenInReserveSrc)
        {
            if (!market.HasValue())
            {
                throw new ArgumentNullException(nameof(market));
            }

            if (!tokenOutAmount.HasValue())
            {
                throw new ArgumentNullException(nameof(tokenOutAmount));
            }

            if (!tokenOutReserveCrs.HasValue())
            {
                throw new ArgumentNullException(nameof(tokenOutReserveCrs));
            }

            if (!tokenOutReserveSrc.HasValue())
            {
                throw new ArgumentNullException(nameof(tokenOutReserveSrc));
            }

            if (!tokenInReserveCrs.HasValue())
            {
                throw new ArgumentNullException(nameof(tokenInReserveCrs));
            }

            if (!tokenInReserveSrc.HasValue())
            {
                throw new ArgumentNullException(nameof(tokenInReserveSrc));
            }

            Market = market;
            TokenOutAmount = tokenOutAmount;
            TokenOutReserveCrs = tokenOutReserveCrs;
            TokenOutReserveSrc = tokenOutReserveSrc;
            TokenInReserveCrs = tokenInReserveCrs;
            TokenInReserveSrc = tokenInReserveSrc;
        }

        public string Market { get; }
        public string TokenOutAmount { get; }
        public string TokenOutReserveCrs { get; }
        public string TokenOutReserveSrc { get; }
        public string TokenInReserveCrs { get; }
        public string TokenInReserveSrc { get; }
    }
}
