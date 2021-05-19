using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Addresses;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses
{
    public class SelectAddressMiningByMiningPoolIdAndOwnerQuery : FindQuery<AddressMining>
    {
        public SelectAddressMiningByMiningPoolIdAndOwnerQuery(long miningPoolId, string owner, bool findOrThrow = true) : base(findOrThrow)
        {
            if (miningPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(miningPoolId));
            }

            if (!owner.HasValue())
            {
                throw new ArgumentNullException(nameof(owner));
            }

            MiningPoolId = miningPoolId;
            Owner = owner;
        }
        
        public long MiningPoolId { get; }
        public string Owner { get; }
    }
}