using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Addresses;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses
{
    public class SelectAddressAllowancesByOwnerWithFilterQuery : IRequest<IEnumerable<AddressAllowance>>
    {
        public SelectAddressAllowancesByOwnerWithFilterQuery(string owner, string spender = "", long tokenId = 0)
        {
            if (!owner.HasValue()) throw new ArgumentNullException(nameof(owner), "Owner address must be set.");

            Owner = owner;
            Spender = spender;
            TokenId = tokenId;
        }

        public string Owner { get; }
        public string Spender { get; }
        public long TokenId { get; }
    }
}
