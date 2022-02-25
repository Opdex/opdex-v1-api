using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Deployers;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;

namespace Opdex.Platform.Application.EntryHandlers.Transactions;

public class ProcessCoreDeploymentTransactionCommandHandler : IRequestHandler<ProcessCoreDeploymentTransactionCommand, Unit>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessCoreDeploymentTransactionCommandHandler> _logger;

    public ProcessCoreDeploymentTransactionCommandHandler(IMediator mediator, ILogger<ProcessCoreDeploymentTransactionCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Unit> Handle(ProcessCoreDeploymentTransactionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var transaction = await _mediator.Send(new RetrieveCirrusTransactionByHashQuery(request.TxHash), CancellationToken.None);

            if (transaction is null || transaction.Id > 0) return Unit.Value;

            // Deployments can have block gaps between transactions. Create all blocks in from our best block to the current block
            // that the Core deployment transaction hash is within.
            var bestBlock = await _mediator.Send(new GetBestBlockReceiptQuery(), CancellationToken.None);
            var txHashBlockHeight = await _mediator.Send(new RetrieveCirrusBlockHashByHeightQuery(transaction.BlockHeight), CancellationToken.None);
            var hashBlock = await _mediator.Send(new RetrieveCirrusBlockReceiptByHashQuery(txHashBlockHeight, findOrThrow: true), CancellationToken.None);

            while (bestBlock.Height <= hashBlock.Height && bestBlock.NextBlockHash.HasValue)
            {
                await _mediator.Send(new MakeBlockCommand(bestBlock.Height, bestBlock.Hash, bestBlock.Time, bestBlock.MedianTime), CancellationToken.None);
                bestBlock = await _mediator.Send(new RetrieveCirrusBlockReceiptByHashQuery(bestBlock.NextBlockHash.Value, findOrThrow: true), CancellationToken.None);
            }

            await _mediator.Send(new CreateCrsTokenSnapshotsCommand(hashBlock.MedianTime, transaction.BlockHeight), CancellationToken.None);
            await _mediator.Send(new CreateDeployerCommand(transaction.NewContractAddress, transaction.From, transaction.BlockHeight), CancellationToken.None);
            await _mediator.Send(new MakeTransactionCommand(transaction), CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failure processing deployer deployment");
        }

        return Unit.Value;
    }
}
