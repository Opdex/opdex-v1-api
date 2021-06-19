using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Abstractions.Commands.Deployers;
using Opdex.Platform.Application.Abstractions.Commands.Markets;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;

namespace Opdex.Platform.Application.EntryHandlers.Transactions
{
    public class ProcessDeployerDeploymentTransactionCommandHandler : IRequestHandler<ProcessDeployerDeploymentTransactionCommand, Unit>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessDeployerDeploymentTransactionCommandHandler> _logger;

        public ProcessDeployerDeploymentTransactionCommandHandler(IMediator mediator, ILogger<ProcessDeployerDeploymentTransactionCommandHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(ProcessDeployerDeploymentTransactionCommand request, CancellationToken cancellationToken)
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
                var localBlockQuery = new RetrieveBlockByHeightQuery(transaction.BlockHeight, findOrThrow: false);
                var block = await _mediator.Send(localBlockQuery, CancellationToken.None);

                if (block == null)
                {
                    // Get transaction block hash
                    var blockHashQuery = new RetrieveCirrusBlockHashByHeightQuery(transaction.BlockHeight);
                    var blockHash = await _mediator.Send(blockHashQuery, CancellationToken.None);

                    // Get block by hash
                    var blockQuery = new RetrieveCirrusBlockByHashQuery(blockHash);
                    var blockReceiptDto = await _mediator.Send(blockQuery, CancellationToken.None);

                    // Make block
                    var blockCommand = new MakeBlockCommand(blockReceiptDto.Height, blockReceiptDto.Hash, blockReceiptDto.Time, blockReceiptDto.MedianTime);
                    var blockCreated = await _mediator.Send(blockCommand, CancellationToken.None);
                }

                // No duplicate attempts to create the same deployer
                var deployerQuery = new RetrieveDeployerByAddressQuery(transaction.NewContractAddress, findOrThrow: false);
                var deployer = await _mediator.Send(deployerQuery, CancellationToken.None) ??
                               new Deployer(transaction.NewContractAddress, transaction.From, transaction.BlockHeight);

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