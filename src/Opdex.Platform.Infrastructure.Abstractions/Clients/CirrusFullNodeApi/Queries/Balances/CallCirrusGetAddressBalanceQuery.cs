using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Balances
{
    /// <summary>
    /// A query to get a CRS balance of an address.
    /// </summary>
    public class CallCirrusGetAddressBalanceQuery : FindQuery<ulong>
    {
        /// <summary>
        /// Constructor building the get address balance query.
        /// </summary>
        /// <param name="address">The wallet address to get the balance of.</param>
        /// <param name="findOrThrow">Flag to find or throw if not found.</param>
        /// <exception cref="ArgumentNullException">Thrown for invalid query arguments.</exception>
        public CallCirrusGetAddressBalanceQuery(Address address, bool findOrThrow = true) : base(findOrThrow)
        {
            Address = address != Address.Empty
                ? address
                : throw new ArgumentNullException(nameof(address), "A wallet address must be provided.");
        }

        public Address Address { get; }
    }
}
