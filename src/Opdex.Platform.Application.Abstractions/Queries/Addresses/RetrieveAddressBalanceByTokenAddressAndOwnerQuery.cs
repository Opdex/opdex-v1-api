using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Addresses;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Addresses
{
    public class RetrieveAddressBalanceByTokenAddressAndOwnerQuery : FindQuery<AddressBalance>
    {
        public RetrieveAddressBalanceByTokenAddressAndOwnerQuery(string tokenAddress, string owner, bool findOrThrow = true) : base(findOrThrow)
        {
            TokenAddress = tokenAddress.HasValue() ? tokenAddress : throw new ArgumentNullException(nameof(tokenAddress), "Token address must be set.");
            Owner = owner.HasValue() ? owner : throw new ArgumentNullException(nameof(owner), "Owner address must be set.");
        }

        public string TokenAddress { get; }
        public string Owner { get; }
    }
}
