using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Blocks;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Blocks;

public class GetBlockReceiptAtChainSplitCommandHandler : IRequestHandler<GetBlockReceiptAtChainSplitCommand, BlockReceipt>
{
    public const int MaxReorg = 10_800;

    private readonly ILogger<GetBlockReceiptAtChainSplitCommandHandler> _logger;
    private readonly IMediator _mediator;

    public GetBlockReceiptAtChainSplitCommandHandler(ILogger<GetBlockReceiptAtChainSplitCommandHandler> logger, IMediator mediator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
        while (bestBlock is null)
        {
            currentBlock = await _mediator.Send(new RetrieveBlockByHeightQuery(currentIndexedHeight - ++reorgLength), cancellationToken);
            bestBlock = await _mediator.Send(new RetrieveCirrusBlockReceiptByHashQuery(currentBlock.Hash, findOrThrow: false), cancellationToken);
        }

        using (_logger.BeginScope(new Dictionary<string, object>()
            {
               { "BlockHeight", bestBlock.Height },
               { "ReorgLength", reorgLength },
            }))
        {
            _logger.LogInformation("Chain split detected");
        }

        return bestBlock;
    }
}
