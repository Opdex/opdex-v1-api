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

namespace Opdex.Platform.Application.EntryHandlers.Transactions
{
    public class ProcessCoreDeploymentTransactionCommandHandler : IRequestHandler<ProcessCoreDeploymentTransactionCommand, Unit>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessCoreDeploymentTransactionCommandHandler> _logger;

        public ProcessCoreDeploymentTransactionCommandHandler(IMediator mediator, ILogger<ProcessCoreDeploymentTransactionCommandHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(ProcessCoreDeploymentTransactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var transaction = await _mediator.Send(new RetrieveTransactionByHashQuery(request.TxHash, findOrThrow: false), cancellationToken) ??
                                  await _mediator.Send(new RetrieveCirrusTransactionByHashQuery(request.TxHash), CancellationToken.None);

                if (transaction == null)
                {
                    return Unit.Value;
                }

                // Hosted environments would not be null, local environments would be null
                var block = await _mediator.Send(new RetrieveBlockByHeightQuery(transaction.BlockHeight, findOrThrow: false));
                var blockTime = block?.MedianTime;

                if (block == null)
                {
                    // Get transaction block hash
                    var blockHash = await _mediator.Send(new RetrieveCirrusBlockHashByHeightQuery(transaction.BlockHeight));

                    // Get block by hash
                    var blockReceipt = await _mediator.Send(new RetrieveCirrusBlockByHashQuery(blockHash));

                    blockTime = blockReceipt.MedianTime;

                    // Make block
                    await _mediator.Send(new MakeBlockCommand(blockReceipt.Height, blockReceipt.Hash, blockReceipt.Time, blockReceipt.MedianTime));
                }

                await _mediator.Send(new CreateCrsTokenSnapshotsCommand(blockTime.Value));

                var deployerId = await _mediator.Send(new CreateDeployerCommand(transaction.NewContractAddress, transaction.From, transaction.BlockHeight, isUpdate: false));

                if (transaction.Id == 0)
                {
                    await _mediator.Send(new MakeTransactionCommand(transaction), CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failure processing deployer deployment.");
            }

            return Unit.Value;
        }
    }
}
