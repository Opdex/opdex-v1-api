using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Addresses;

namespace Opdex.Platform.Application.Abstractions.Queries.Addresses
{
    public class RetrieveAddressMiningByMiningPoolIdAndOwnerQuery : IRequest<AddressMining>
    {
        public RetrieveAddressMiningByMiningPoolIdAndOwnerQuery(long miningPoolId, string owner)
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