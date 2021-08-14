using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Abstractions.Commands.Deployers;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Domain.Models;

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
                // Hosted environments would have indexed transaction already, local would need to reach out to Cirrus to get receipt
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

                // No duplicate attempts to create the same deployer
                var deployer = await _mediator.Send(new RetrieveDeployerByAddressQuery(transaction.NewContractAddress, findOrThrow: false))
                               ?? new Deployer(transaction.NewContractAddress, transaction.From, transaction.BlockHeight);

                var deployerId = deployer.Id;
                if (deployerId == 0)
                {
                    var deployerCommand = new MakeDeployerCommand(deployer);
                    deployerId = await _mediator.Send(deployerCommand, CancellationToken.None);
                }

                // In hosted environments, transaction would've already been inserted. Local environments need to persist
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
