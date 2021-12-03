using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Staking;

/// <summary>
/// Retrieves a staking position of a particular address in a liquidity pool
/// </summary>
public class GetStakingPositionByPoolQuery : IRequest<StakingPositionDto>
{
    public GetStakingPositionByPoolQuery(Address staker, Address liquidityPool)
    {
        Address = staker != Address.Empty ? staker : throw new ArgumentNullException(nameof(staker), "Staker address must be set.");
        LiquidityPoolAddress = liquidityPool != Address.Empty ? liquidityPool : throw new ArgumentNullException(nameof(liquidityPool), "Liquidity pool address must be set.");
    }

    public Address Address { get; }
    public Address LiquidityPoolAddress { get; }
}