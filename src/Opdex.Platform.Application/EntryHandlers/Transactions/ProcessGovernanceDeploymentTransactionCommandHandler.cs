using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Governances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;

namespace Opdex.Platform.Application.EntryHandlers.Transactions
{
    public class ProcessGovernanceDeploymentTransactionCommandHandler : IRequestHandler<ProcessGovernanceDeploymentTransactionCommand, Unit>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessGovernanceDeploymentTransactionCommandHandler> _logger;

        public ProcessGovernanceDeploymentTransactionCommandHandler(IMediator mediator, ILogger<ProcessGovernanceDeploymentTransactionCommandHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(ProcessGovernanceDeploymentTransactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Todo: Evaluate, rewrite, and/or reword all of these types of comments, these processes should be idempotent
                // Hosted environments would have indexed transaction already, local would need to reach out to Cirrus to get receipt
                var transaction = await _mediator.Send(new RetrieveTransactionByHashQuery(request.TxHash, findOrThrow: false)) ??
                                  await _mediator.Send(new RetrieveCirrusTransactionByHashQuery(request.TxHash));

                if (transaction == null)
                {
                    return Unit.Value;
                }

                // Hosted environments would not be null, local environments would be null
                var block = await _mediator.Send(new RetrieveBlockByHeightQuery(transaction.BlockHeight, findOrThrow: false));

                if (block == null)
                {
                    // Get transaction block hash
                    var blockHash = await _mediator.Send(new RetrieveCirrusBlockHashByHeightQuery(transaction.BlockHeight));

                    // Get block by hash
                    var blockReceiptDto = await _mediator.Send(new RetrieveCirrusBlockReceiptByHashQuery(blockHash, findOrThrow: true));

                    // Make block
                    await _mediator.Send(new MakeBlockCommand(blockReceiptDto.Height, blockReceiptDto.Hash,
                                                              blockReceiptDto.Time, blockReceiptDto.MedianTime));
                }

                // Insert Staking Token
                var stakingTokenId = await _mediator.Send(new CreateTokenCommand(transaction.NewContractAddress, transaction.BlockHeight));

                // Get token summary
                var stakingTokenSummary = await _mediator.Send(new RetrieveStakingTokenContractSummaryQuery(transaction.NewContractAddress,
                                                                                                            transaction.BlockHeight,
                                                                                                            includeVault: true,
                                                                                                            includeMiningGovernance: true));

                // Get and/or create vault
                var vault = await _mediator.Send(new CreateVaultCommand(stakingTokenSummary.Vault.GetValueOrDefault(),
                                                                        stakingTokenId, transaction.From, transaction.BlockHeight));

                // Get and/or create mining governance
                var governanceId = await _mediator.Send(new CreateMiningGovernanceCommand(stakingTokenSummary.MiningGovernance.GetValueOrDefault(),
                                                                                          stakingTokenId, transaction.BlockHeight));

                if (transaction.Id == 0)
                {
                    await _mediator.Send(new MakeTransactionCommand(transaction));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failure processing mined token and governance deployment.");
            }

            return Unit.Value;
        }
    }
}
