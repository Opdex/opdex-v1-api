using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Addresses;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses
{
    public class SelectAddressAllowanceByTokenIdAndOwnerAndSpenderQuery : FindQuery<AddressAllowance>
    {
        public SelectAddressAllowanceByTokenIdAndOwnerAndSpenderQuery(long tokenId, string owner, string spender, bool findOrThrow = true) : base(findOrThrow)
        {
            if (tokenId <= 0) throw new ArgumentOutOfRangeException(nameof(tokenId), "Token id must be greater than zero.");
            if (!owner.HasValue()) throw new ArgumentNullException(nameof(owner), "Owner address must be set.");
            if (!spender.HasValue()) throw new ArgumentNullException(nameof(spender), "Spender address must be set.");

            TokenId = tokenId;
            Owner = owner;
            Spender = spender;
        }

        public long TokenId { get; }
        public string Owner { get; }
        public string Spender { get; }
    }
}
