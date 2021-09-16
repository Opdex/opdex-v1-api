using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Addresses;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Addresses.Balances
{
    /// <summary>
    /// Retrieves the balance of a wallet address for a specified token.
    /// </summary>
    public class RetrieveAddressBalanceByOwnerAndTokenQuery : FindQuery<AddressBalance>
    {
        /// <summary>
        /// Constructor to initialize the get balance retrieval request.
        /// </summary>
        /// <param name="owner">The wallet address to check the balance of.</param>
        /// <param name="tokenId">Optional tokenId to lookup the balance by.</param>
        /// <param name="tokenAddress">Optional token address to lookup the balance by.</param>
        /// <param name="findOrThrow">Defaulted to true, optionally throw not found exception when applicable.</param>
        /// <exception cref="ArgumentNullException">Argument null exception for invalid request arguments.</exception>
        /// <exception cref="ArgumentException">Argument exception thrown when tokenId and tokenAddress are both invalid.</exception>
        public RetrieveAddressBalanceByOwnerAndTokenQuery(Address owner, long? tokenId = null, Address? tokenAddress = null, bool findOrThrow = true) : base(findOrThrow)
        {
            Owner = owner != Address.Empty ? owner : throw new ArgumentNullException(nameof(owner), "Owner address must be set.");

            if (!(tokenId.HasValue && tokenId.Value > 0) && tokenAddress == Address.Empty)
            {
                throw new ArgumentException("Either a tokenId or a tokenAddress must be provided.");
            }

            TokenAddress = tokenAddress ?? Address.Empty;
            TokenId = tokenId;
        }

        public Address TokenAddress { get; }
        public long? TokenId { get; }
        public Address Owner { get; }
    }
}
