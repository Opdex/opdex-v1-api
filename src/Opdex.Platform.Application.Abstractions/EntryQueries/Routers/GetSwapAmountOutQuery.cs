using MediatR;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Routers
{
    /// <summary>
    /// Retrieves an estimate for the amount of tokens that will be output for a swap.
    /// </summary>
    public class GetSwapAmountOutQuery : IRequest<FixedDecimal>
    {
        /// <summary>
        /// Creates a query that will retrieve an estimate for the amount of tokens that will be output for a swap.
        /// </summary>
        /// <param name="market">The address of the market.</param>
        /// <param name="tokenIn">The address of the input token, or CRS.</param>
        /// <param name="tokenInAmount">The amount of tokens input.</param>
        /// <param name="tokenOut">The address of the output token, or CRS.</param>
        public GetSwapAmountOutQuery(Address market, Address tokenIn, FixedDecimal tokenInAmount, Address tokenOut)
        {
            if (market == Address.Empty) throw new ArgumentNullException(nameof(market), "Market must be non-empty address.");
            if (tokenIn == Address.Empty) throw new ArgumentNullException(nameof(tokenIn), "Token in must be non-empty address.");
            if (tokenInAmount <= FixedDecimal.Zero) throw new ArgumentOutOfRangeException(nameof(tokenInAmount), "Token in amount must be greater than zero.");
            if (tokenOut == Address.Empty) throw new ArgumentNullException(nameof(tokenOut), "Token out must be non-empty address.");

            if (tokenIn == tokenOut) throw new ArgumentException("Cannot make a swap with the same input and output token.");

            Market = market;
            TokenIn = tokenIn;
            TokenInAmount = tokenInAmount;
            TokenOut = tokenOut;
        }

        public Address Market { get; }
        public Address TokenIn { get; }
        public FixedDecimal TokenInAmount { get; }
        public Address TokenOut { get; }

        public bool IsSingleHopQuery => TokenIn == Address.Cirrus || TokenOut == Address.Cirrus;
    }
}
