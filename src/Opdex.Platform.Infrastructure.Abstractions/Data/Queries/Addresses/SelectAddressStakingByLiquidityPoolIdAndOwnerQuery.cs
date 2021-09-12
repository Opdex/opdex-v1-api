using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Addresses;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses
{
    public class SelectAddressStakingByLiquidityPoolIdAndOwnerQuery : FindQuery<AddressStaking>
    {
        public SelectAddressStakingByLiquidityPoolIdAndOwnerQuery(long liquidityPoolId, Address owner, bool findOrThrow = true) : base(findOrThrow)
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId));
            }

            if (owner == Address.Empty)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            LiquidityPoolId = liquidityPoolId;
            Owner = owner;
        }

        public long LiquidityPoolId { get; }
        public Address Owner { get; }
    }
}
