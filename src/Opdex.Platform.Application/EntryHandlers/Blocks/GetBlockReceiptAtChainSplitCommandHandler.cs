using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Blocks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Blocks;

public class GetBlockReceiptAtChainSplitCommandHandler : IRequestHandler<GetBlockReceiptAtChainSplitCommand, BlockReceipt>
{
    public const int MaxReorg = 10_800;

    private readonly IMediator _mediator;

    public GetBlockReceiptAtChainSplitCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<BlockReceipt> Handle(GetBlockReceiptAtChainSplitCommand request, CancellationToken cancellationToken)
    {
        BlockReceipt bestBlock = null;

        // Get our latest synced block from the database
        var currentBlock = await _mediator.Send(new RetrieveLatestBlockQuery(findOrThrow: true), cancellationToken);
        var currentIndexedHeight = currentBlock.Height;

        // Walk backward through our database blocks until we find one that can be found at the FN
        ulong reorgLength = 0;
        while (bestBlock is null && ++reorgLength < MaxReorg)
        {
            currentBlock = await _mediator.Send(new RetrieveBlockByHeightQuery(currentIndexedHeight - reorgLength), cancellationToken);
            bestBlock = await _mediator.Send(new RetrieveCirrusBlockReceiptByHashQuery(currentBlock.Hash, findOrThrow: false), cancellationToken);
        }

        if (bestBlock is null) throw new MaximumReorgException();

        return bestBlock;
    }
}
