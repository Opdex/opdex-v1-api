using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Addresses;

namespace Opdex.Platform.Application.Abstractions.Queries.Addresses
{
    public class RetrieveAddressStakingByLiquidityPoolIdAndOwnerQuery : IRequest<AddressStaking>
    {
        public RetrieveAddressStakingByLiquidityPoolIdAndOwnerQuery(long liquidityPoolId, string owner)
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