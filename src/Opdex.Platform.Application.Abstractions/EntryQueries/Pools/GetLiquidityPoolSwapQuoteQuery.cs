using System;
using MediatR;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Pools
{
    public class GetLiquidityPoolSwapQuoteQuery : IRequest<string>
    {
        public GetLiquidityPoolSwapQuoteQuery(string tokenIn, string tokenOut, string tokenInAmount, string tokenOutAmount, string market)
        {
            if (!tokenIn.HasValue() || !tokenOut.HasValue())
            {
                throw new ArgumentException("The token in or token out address must not be null.");
            }

            if (!market.HasValue())
            {
                throw new ArgumentNullException(nameof(market));
            }

            if (!tokenInAmount.IsValidDecimalNumber())
            {
                throw new ArgumentException("Token in amount must be a valid decimal number.", nameof(tokenInAmount));
            }

            if (!tokenOutAmount.IsValidDecimalNumber())
            {
                throw new ArgumentException("Token out amount must be a valid decimal number.", nameof(tokenOutAmount));
            }

            TokenIn = tokenIn;
            TokenOut = tokenOut;
            TokenInAmount = tokenInAmount;
            TokenOutAmount = tokenOutAmount;
            Market = market;
        }

        public string TokenIn { get; }
        public string TokenOut { get; }
        public string TokenInAmount { get; }
        public string TokenOutAmount { get; }
        public string Market { get; }
    }
}
