using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Addresses;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses
{
    public class SelectAddressBalanceByTokenAddressAndOwnerQuery : FindQuery<AddressBalance>
    {
        public SelectAddressBalanceByTokenAddressAndOwnerQuery(string tokenAddress, string owner, bool findOrThrow = true) : base(findOrThrow)
        {
            TokenAddress = tokenAddress.HasValue() ? tokenAddress : throw new ArgumentNullException(nameof(tokenAddress), "Token address must be set.");
            Owner = owner.HasValue() ? owner : throw new ArgumentNullException(nameof(owner), "Owner address must be set.");
        }

        public string TokenAddress { get; }
        public string Owner { get; }
    }
}
