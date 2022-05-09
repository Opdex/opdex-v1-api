using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Staking;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Staking;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Balances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Addresses.Staking;

public class CreateRefreshStakingPositionCommandHandler : IRequestHandler<CreateRefreshStakingPositionCommand, StakingPositionDto>
{
    private readonly IMediator _mediator;
    private readonly IModelAssembler<AddressStaking, StakingPositionDto> _stakingPositionAssembler;
    private readonly ILogger<CreateRefreshStakingPositionCommandHandler> _logger;

    public CreateRefreshStakingPositionCommandHandler(IMediator mediator, IModelAssembler<AddressStaking, StakingPositionDto> stakingPositionAssembler,
        ILogger<CreateRefreshStakingPositionCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _stakingPositionAssembler = stakingPositionAssembler ?? throw new ArgumentNullException(nameof(stakingPositionAssembler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<StakingPositionDto> Handle(CreateRefreshStakingPositionCommand request, CancellationToken cancellationToken)
    {
        var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(request.LiquidityPool), cancellationToken);
        var block = await _mediator.Send(new RetrieveLatestBlockQuery(), cancellationToken);

        var position = await _mediator.Send(new RetrieveAddressStakingByLiquidityPoolIdAndOwnerQuery(liquidityPool.Id, request.Staker), cancellationToken)
                       ?? new AddressStaking(liquidityPool.Id, request.Staker, UInt256.Zero, block.Height);

        // Reject making out of date updates to position
        if (position.ModifiedBlock > block.Height) return await _stakingPositionAssembler.Assemble(position);

        var updatedWeight = await _mediator.Send(new CallCirrusGetStakingWeightForAddressQuery(liquidityPool.Address, request.Staker, block.Height), CancellationToken.None);
        position.SetWeight(updatedWeight, block.Height);
        await _mediator.Send(new MakeAddressStakingCommand(position), CancellationToken.None);

        // Update ODX balance
        var market = await _mediator.Send(new RetrieveMarketByIdQuery(liquidityPool.MarketId, false), cancellationToken);
        if (market is null) _logger.LogError("Market could not be found containing pool {LiquidityPool}", liquidityPool.Address);
        var minedToken = await _mediator.Send(new RetrieveTokenByIdQuery(market!.StakingTokenId, false), cancellationToken);
        await _mediator.Send(new CreateRefreshAddressBalanceCommand(request.Staker, minedToken.Address), cancellationToken);

        return await _stakingPositionAssembler.Assemble(position);
    }
}
