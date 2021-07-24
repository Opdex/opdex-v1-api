using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Addresses;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses
{
    public class SelectAddressBalanceByOwnerAndTokenIdQuery : FindQuery<AddressBalance>
    {
        public SelectAddressBalanceByOwnerAndTokenIdQuery(string owner, long tokenId, bool findOrThrow = true) : base(findOrThrow)
        {
            if (!owner.HasValue())
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId));
            }

            Owner = owner;
            TokenId = tokenId;
        }

        public string Owner { get; }
        public long TokenId { get; }
    }
}
