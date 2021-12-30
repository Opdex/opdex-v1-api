using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets;

/// <summary>
/// Retrieve a market by its contract address.
/// </summary>
public class RetrieveMarketByAddressQuery : FindQuery<Market>
{
    /// <summary>
    /// Create a retrieve market by address query.
    /// </summary>
    /// <param name="address">The address of the market to fetch.</param>
    /// <param name="findOrThrow">True to throw an exception if a market is not found, else null.</param>
    public RetrieveMarketByAddressQuery(Address address, bool findOrThrow = true) : base(findOrThrow)
    {
        Address = address != Address.Empty ? address : throw new ArgumentNullException(nameof(address));
    }

    public Address Address { get; }
}
