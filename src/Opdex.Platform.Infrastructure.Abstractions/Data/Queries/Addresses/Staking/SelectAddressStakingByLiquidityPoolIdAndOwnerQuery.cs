using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Addresses;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Staking;

public class SelectAddressStakingByLiquidityPoolIdAndOwnerQuery : FindQuery<AddressStaking>
{
    public SelectAddressStakingByLiquidityPoolIdAndOwnerQuery(ulong liquidityPoolId, Address owner, bool findOrThrow = true) : base(findOrThrow)
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

    public ulong LiquidityPoolId { get; }
    public Address Owner { get; }
}