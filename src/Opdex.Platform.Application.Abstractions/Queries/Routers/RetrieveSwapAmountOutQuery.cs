using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.Tokens;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Routers;

/// <summary>
/// Retrieves the amount of tokens that will be output, for any type of swap, based on an amount of input tokens.
/// </summary>
public class RetrieveSwapAmountOutQuery : IRequest<UInt256>
{
    /// <summary>
    /// Creates a request to retrieve the amount of tokens that will be output, for any type of swap, based on an amount of input tokens.
    /// </summary>
    /// <param name="router">The market router.</param>
    /// <param name="tokenIn">The input token.</param>
    /// <param name="tokenInAmount">The input token amount.</param>
    /// <param name="tokenOut">The output token.</param>
    public RetrieveSwapAmountOutQuery(MarketRouter router, Token tokenIn, UInt256 tokenInAmount, Token tokenOut)
    {
        if (router is null) throw new ArgumentNullException(nameof(router), "The router must not be null.");
        if (tokenIn is null) throw new ArgumentNullException(nameof(tokenIn), "The input token must not be null.");
        if (tokenInAmount == UInt256.Zero) throw new ArgumentOutOfRangeException(nameof(tokenInAmount), "Token input amount must be greater than zero.");
        if (tokenOut is null) throw new ArgumentNullException(nameof(tokenOut), "The output token must not be null.");

        if (tokenIn.Address == tokenOut.Address) throw new ArgumentException("Token in and token out must not be the same.");

        Router = router;
        TokenIn = tokenIn;
        TokenInAmount = tokenInAmount;
        TokenOut = tokenOut;
    }

    public MarketRouter Router { get; }
    public Token TokenIn { get; }
    public UInt256 TokenInAmount { get; }
    public Token TokenOut { get; }

    public bool IsSingleHopQuery => TokenIn.Address == Address.Cirrus || TokenOut.Address == Address.Cirrus;
}