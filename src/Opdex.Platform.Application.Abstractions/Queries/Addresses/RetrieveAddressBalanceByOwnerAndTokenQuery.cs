using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Addresses;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Addresses
{
    public class RetrieveAddressBalanceByOwnerAndTokenQuery : FindQuery<AddressBalance>
    {
        public RetrieveAddressBalanceByOwnerAndTokenQuery(string owner, long? tokenId = null, string tokenAddress = null, bool findOrThrow = true) : base(findOrThrow)
        {
            Owner = owner.HasValue() ? owner : throw new ArgumentNullException(nameof(owner), "Owner address must be set.");

            if (!(tokenId.HasValue && tokenId.Value > 0) && !tokenAddress.HasValue())
            {
                throw new ArgumentException("Either a tokenId or a tokenAddress must be provided.");
            }

            TokenAddress = tokenAddress;
            TokenId = tokenId;
        }

        public string TokenAddress { get; }
        public long? TokenId { get; }
        public string Owner { get; }
    }
}
