using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Addresses;

namespace Opdex.Platform.Application.Abstractions.Queries.Addresses
{
    public class RetrieveAddressMiningByMiningPoolIdAndOwnerQuery : FindQuery<AddressMining>
    {
        public RetrieveAddressMiningByMiningPoolIdAndOwnerQuery(long miningPoolId, Address owner, bool findOrThrow = true) : base(findOrThrow)
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

        public long MiningPoolId { get; }
        public Address Owner { get; }
    }
}
