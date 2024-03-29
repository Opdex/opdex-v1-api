using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools;

/// <summary>
/// Retrieves the CRS and SRC reserve amounts in a liquidity pool.
/// </summary>
public class CallCirrusGetOpdexLiquidityPoolReservesQuery : IRequest<ReservesReceipt>
{
    /// <summary>
    /// Creates a request to retrieve the CRS and SRC reserve amounts in a liquidity pool.
    /// </summary>
    /// <param name="address">The address of the liquidity pool.</param>
    public CallCirrusGetOpdexLiquidityPoolReservesQuery(Address address)
    {
        if (address == Address.Empty)
        {
            throw new ArgumentNullException(nameof(address), "The liquidity pool address must be set.");
        }

        Address = address;
    }

    public Address Address { get; }
}