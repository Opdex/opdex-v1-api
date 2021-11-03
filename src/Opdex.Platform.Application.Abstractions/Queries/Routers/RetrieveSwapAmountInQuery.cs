using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.Tokens;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Routers
{
    /// <summary>
    /// Creates a request to retrieve the amount of tokens that need to be input, for any type of swap, based on an amount of output tokens.
    /// </summary>
    public class RetrieveSwapAmountInQuery : IRequest<UInt256>
    {
        /// <summary>
        /// Creates a request to retrieve the amount of tokens that need to be input, for any type of swap, based on an amount of output tokens.
        /// </summary>
        /// <param name="router">The market router.</param>
        /// <param name="tokenIn">The input token.</param>
        /// <param name="tokenOut">The output token.</param>
        /// <param name="tokenOutAmount">The output token amount.</param>
        public RetrieveSwapAmountInQuery(MarketRouter router, Token tokenIn, Token tokenOut, UInt256 tokenOutAmount)
        {
            if (router is null) throw new ArgumentNullException(nameof(router), "The router must not be null.");
            if (tokenIn is null) throw new ArgumentNullException(nameof(tokenIn), "The input token must not be null.");
            if (tokenOut is null) throw new ArgumentNullException(nameof(tokenOut), "The output token must not be null.");
            if (tokenOutAmount == UInt256.Zero) throw new ArgumentOutOfRangeException(nameof(tokenOutAmount), "Token output amount must be greater than zero.");

            if (tokenIn.Address == tokenOut.Address) throw new ArgumentException("Token in and token out must not be the same.");

            Router = router;
            TokenIn = tokenIn;
            TokenOut = tokenOut;
            TokenOutAmount = tokenOutAmount;
        }

        public MarketRouter Router { get; }
        public Token TokenIn { get; }
        public Token TokenOut { get; }
        public UInt256 TokenOutAmount { get; }

        public bool IsSingleHopQuery => TokenIn.Address == Address.Cirrus || TokenOut.Address == Address.Cirrus;
    }
}
