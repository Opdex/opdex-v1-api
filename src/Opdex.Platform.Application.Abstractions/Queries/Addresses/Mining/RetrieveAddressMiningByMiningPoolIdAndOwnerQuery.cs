using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Addresses;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Addresses.Mining
{
    public class RetrieveAddressMiningByMiningPoolIdAndOwnerQuery : FindQuery<AddressMining>
    {
        public RetrieveAddressMiningByMiningPoolIdAndOwnerQuery(ulong miningPoolId, Address owner, bool findOrThrow = true) : base(findOrThrow)
        {
            if (miningPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(miningPoolId));
            }

            if (owner == Address.Empty)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            MiningPoolId = miningPoolId;
            Owner = owner;
        }

        public ulong MiningPoolId { get; }
        public Address Owner { get; }
    }
}
