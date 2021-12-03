using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.LiquidityQuotes;

/// <summary>
/// Retrieves the amount of token B required to provide liquidity to a pool, based on the pool reserves and the token A input amount.
/// </summary>
public class CallCirrusGetLiquidityAmountInQuoteQuery : IRequest<UInt256>
{
    /// <summary>
    /// Creates a request to retrieve the amount of token B required to provide liquidity to a pool.
    /// </summary>
    /// <param name="amountA">The amount of token A provided to the pool.</param>
    /// <param name="reserveA">The reserve amount of token A in the pool.</param>
    /// <param name="reserveB">The reserve amount of token B in the pool.</param>
    /// <param name="router">The address of the router contract.</param>
    public CallCirrusGetLiquidityAmountInQuoteQuery(UInt256 amountA, UInt256 reserveA, UInt256 reserveB, Address router)
    {
        if (router == Address.Empty)
        {
            throw new ArgumentNullException(nameof(router), "The router address must be set.");
        }

        if (amountA == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amountA), "Amount A must be greater than 0.");
        }

        AmountA = amountA;
        ReserveA = reserveA;
        ReserveB = reserveB;
        Router = router;
    }

    public UInt256 AmountA { get; }
    public UInt256 ReserveA { get; }
    public UInt256 ReserveB { get; }
    public Address Router { get; }
}