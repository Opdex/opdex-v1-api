using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Addresses;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses
{
    /// <summary>
    /// Selects an address' balance of an SRC token from the database.
    /// </summary>
    public class SelectAddressBalanceByOwnerAndTokenIdQuery : FindQuery<AddressBalance>
    {
        /// <summary>
        /// Constructor for the select address balance request.
        /// </summary>
        /// <param name="owner">The owner of the tokens wallet address</param>
        /// <param name="tokenId">The Id of the token</param>
        /// <param name="findOrThrow">Defaulted to true, optionally throw if a record is not found.</param>
        /// <exception cref="ArgumentNullException">Thrown when the owner parameter is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the tokenId provided is invalid.</exception>
        public SelectAddressBalanceByOwnerAndTokenIdQuery(string owner, long tokenId, bool findOrThrow = true) : base(findOrThrow)
        {
            if (!owner.HasValue())
            {
                throw new ArgumentNullException(nameof(owner), "Owner must be provided.");
            }

            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId), "TokenId must be provided.");
            }

            Owner = owner;
            TokenId = tokenId;
        }

        public string Owner { get; }
        public long TokenId { get; }
    }
}
