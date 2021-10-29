using MediatR;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Routers
{
    /// <summary>
    /// Retrieves an estimate for the amount of tokens that need to be input for a swap.
    /// </summary>
    public class GetSwapAmountInQuery : IRequest<FixedDecimal>
    {
        /// <summary>
        /// Creates a query that will retrieve an estimate for the amount of tokens that need to be input for a swap.
        /// </summary>
        /// <param name="market">The address of the market.</param>
        /// <param name="tokenIn">The address of the input token, or CRS.</param>
        /// <param name="tokenOut">The address of the output token, or CRS.</param>
        /// <param name="tokenOutAmount">The amount of tokens output.</param>
        public GetSwapAmountInQuery(Address market, Address tokenIn, Address tokenOut, FixedDecimal tokenOutAmount)
        {
            if (market == Address.Empty) throw new ArgumentNullException(nameof(market), "Market must be non-empty address.");
            if (tokenIn == Address.Empty) throw new ArgumentNullException(nameof(tokenIn), "Token in must be non-empty address.");
            if (tokenOut == Address.Empty) throw new ArgumentNullException(nameof(tokenOut), "Token out must be non-empty address.");
            if (tokenOutAmount <= FixedDecimal.Zero) throw new ArgumentOutOfRangeException(nameof(tokenOutAmount), "Token out amount must be greater than zero.");

            if (tokenIn == tokenOut) throw new ArgumentException("Cannot make a swap with the same input and output token.");

            Market = market;
            TokenIn = tokenIn;
            TokenOut = tokenOut;
            TokenOutAmount = tokenOutAmount;
        }

        public Address Market { get; }
        public Address TokenIn { get; }
        public Address TokenOut { get; }
        public FixedDecimal TokenOutAmount { get; }

        public bool IsSingleHopQuery => TokenIn == Address.Cirrus || TokenOut == Address.Cirrus;
    }
}
