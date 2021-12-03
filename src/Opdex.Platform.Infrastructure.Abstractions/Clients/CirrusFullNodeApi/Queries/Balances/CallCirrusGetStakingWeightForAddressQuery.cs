using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Balances;

/// <summary>Query to retrieve the staking weight of an address. </summary>
public class CallCirrusGetStakingWeightForAddressQuery : IRequest<UInt256>
{
    /// <summary>Creates a query to retrieve the staking weight of an address. </summary>
    /// <param name="stakingPool">The address of the staking pool.</param>
    /// <param name="staker">The address of the staker.</param>
    /// <param name="blockHeight">Block height to query at.</param>
    public CallCirrusGetStakingWeightForAddressQuery(Address stakingPool, Address staker, ulong blockHeight)
    {
        if (stakingPool == Address.Empty) throw new ArgumentNullException(nameof(stakingPool), "Staking pool address must be set.");
        if (staker == Address.Empty) throw new ArgumentNullException(nameof(staker), "Staker address must be set.");
        if (blockHeight == 0) throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");

        StakingPool = stakingPool;
        Staker = staker;
        BlockHeight = blockHeight;
    }

    public Address StakingPool { get; }
    public Address Staker { get; }
    public ulong BlockHeight { get; }
}