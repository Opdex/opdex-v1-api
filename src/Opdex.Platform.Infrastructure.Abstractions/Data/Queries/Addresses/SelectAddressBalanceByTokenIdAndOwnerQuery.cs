using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Addresses;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses
{
    public class SelectAddressBalanceByTokenIdAndOwnerQuery : FindQuery<AddressBalance>
    {
        public SelectAddressBalanceByTokenIdAndOwnerQuery(long tokenId, string owner, bool findOrThrow = true) : base(findOrThrow)
        {
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId));
            }

            if (!owner.HasValue())
            {
                throw new ArgumentNullException(nameof(owner));
            }

            TokenId = tokenId;
            Owner = owner;
        }
        
        public long TokenId { get; }
        public string Owner { get; }
    }
}