using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Mining;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Mining;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Balances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Addresses.Mining;

public class CreateRefreshMiningPositionCommandHandler : IRequestHandler<CreateRefreshMiningPositionCommand, MiningPositionDto>
{
    private readonly IMediator _mediator;
    private readonly IModelAssembler<AddressMining, MiningPositionDto> _miningPositionAssembler;

    public CreateRefreshMiningPositionCommandHandler(IMediator mediator, IModelAssembler<AddressMining, MiningPositionDto> miningPositionAssembler)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _miningPositionAssembler = miningPositionAssembler ?? throw new ArgumentNullException(nameof(miningPositionAssembler));
    }

    public async Task<MiningPositionDto> Handle(CreateRefreshMiningPositionCommand request, CancellationToken cancellationToken)
    {
        var miningPool = await _mediator.Send(new RetrieveMiningPoolByAddressQuery(request.MiningPool), cancellationToken);
        var block = await _mediator.Send(new RetrieveLatestBlockQuery(), cancellationToken);

        var position = await _mediator.Send(new RetrieveAddressMiningByMiningPoolIdAndOwnerQuery(miningPool.Id, request.Miner), cancellationToken)
                               ?? new AddressMining(miningPool.Id, request.Miner, UInt256.Zero, block.Height);

        // Reject making out of date updates to position
        if (position.ModifiedBlock > block.Height) return await _miningPositionAssembler.Assemble(position);

        var updatedBalance = await _mediator.Send(new CallCirrusGetMiningBalanceForAddressQuery(miningPool.Address, request.Miner, block.Height), CancellationToken.None);
        position.SetBalance(updatedBalance, block.Height);
        await _mediator.Send(new MakeAddressMiningCommand(position), CancellationToken.None);

        return await _miningPositionAssembler.Assemble(position);
    }
}
