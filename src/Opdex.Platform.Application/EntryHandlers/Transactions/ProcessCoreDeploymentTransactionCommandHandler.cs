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
            var transaction = await _mediator.Send(new RetrieveTransactionByHashQuery(request.TxHash, findOrThrow: false), CancellationToken.None) ??
                              await _mediator.Send(new RetrieveCirrusTransactionByHashQuery(request.TxHash), CancellationToken.None);

            if (transaction is null || transaction.Id > 0) return Unit.Value;

            var block = await _mediator.Send(new RetrieveBlockByHeightQuery(transaction.BlockHeight, findOrThrow: false));
            DateTime blockTime;

            if (block == null)
            {
                // Get transaction block hash
                var blockHash = await _mediator.Send(new RetrieveCirrusBlockHashByHeightQuery(transaction.BlockHeight));

                // Get block by hash
                var blockReceiptDto = await _mediator.Send(new RetrieveCirrusBlockReceiptByHashQuery(blockHash, findOrThrow: true));

                blockTime = blockReceiptDto.MedianTime;

                // Make block
                await _mediator.Send(new MakeBlockCommand(blockReceiptDto.Height, blockReceiptDto.Hash,
                                                          blockReceiptDto.Time, blockReceiptDto.MedianTime));
            }
            else
            {
                blockTime = block.MedianTime;
            }

            await _mediator.Send(new CreateCrsTokenSnapshotsCommand(blockTime, transaction.BlockHeight), CancellationToken.None);
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
