using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Addresses;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses
{
    public class SelectAddressStakingByLiquidityPoolIdAndOwnerQuery : IRequest<AddressStaking>
    {
        public SelectAddressStakingByLiquidityPoolIdAndOwnerQuery(long liquidityPoolId, string owner)
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId));
            }

            if (!owner.HasValue())
            {
                throw new ArgumentNullException(nameof(owner));
            }

            LiquidityPoolId = liquidityPoolId;
            Owner = owner;
        }
        
        public long LiquidityPoolId { get; }
        public string Owner { get; }
    }
}