using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Staking;

public class CreateRefreshStakingPositionCommand : IRequest<StakingPositionDto>
{
    public CreateRefreshStakingPositionCommand(Address staker, Address liquidityPool)
    {
        Staker = staker != Address.Empty ? staker : throw new ArgumentNullException(nameof(staker), "Staker address must be set.");
        LiquidityPool = liquidityPool != Address.Empty ? liquidityPool : throw new ArgumentNullException(nameof(liquidityPool), "Liquidity pool address must be set.");
    }

    public Address Staker { get; }
    public Address LiquidityPool { get; }
}
