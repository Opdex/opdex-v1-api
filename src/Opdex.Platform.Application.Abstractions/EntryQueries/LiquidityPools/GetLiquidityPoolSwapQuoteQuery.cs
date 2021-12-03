using MediatR;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools;

public class GetLiquidityPoolSwapQuoteQuery : IRequest<FixedDecimal>
{
    public GetLiquidityPoolSwapQuoteQuery(Address tokenIn, Address tokenOut, FixedDecimal tokenInAmount, FixedDecimal tokenOutAmount, Address market)
    {
        if (tokenIn == Address.Empty && tokenOut == Address.Empty)
        {
            throw new ArgumentException("The token in or token out address must not be null.");
        }

        if (market == Address.Empty)
        {
            throw new ArgumentNullException(nameof(market));
        }

        // Expected either or, not both
        if (!(tokenInAmount > FixedDecimal.Zero ^ tokenOutAmount > FixedDecimal.Zero))
        {
            throw new ArgumentException("Only token in amount or token out amount can have a value.");
        }

        TokenIn = tokenIn;
        TokenOut = tokenOut;
        TokenInAmount = tokenInAmount;
        TokenOutAmount = tokenOutAmount;
        Market = market;
    }

    public Address TokenIn { get; }
    public Address TokenOut { get; }
    public FixedDecimal TokenInAmount { get; }
    public FixedDecimal TokenOutAmount { get; }
    public Address Market { get; }
}