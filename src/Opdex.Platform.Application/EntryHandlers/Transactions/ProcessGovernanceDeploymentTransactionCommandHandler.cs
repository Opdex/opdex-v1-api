using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningGovernances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;

namespace Opdex.Platform.Application.EntryHandlers.Transactions;

public class ProcessGovernanceDeploymentTransactionCommandHandler : IRequestHandler<ProcessGovernanceDeploymentTransactionCommand, Unit>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessGovernanceDeploymentTransactionCommandHandler> _logger;

    public ProcessGovernanceDeploymentTransactionCommandHandler(IMediator mediator, ILogger<ProcessGovernanceDeploymentTransactionCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Unit> Handle(ProcessGovernanceDeploymentTransactionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var transaction = await _mediator.Send(new RetrieveCirrusTransactionByHashQuery(request.TxHash), CancellationToken.None) ??
                              await _mediator.Send(new RetrieveCirrusTransactionByHashQuery(request.TxHash), CancellationToken.None);

            if (transaction == null || transaction.Id > 0) return Unit.Value;

            // Hosted environments would not be null, local environments would be null
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

            // Insert Staking Token
            var stakingTokenId = await _mediator.Send(new CreateTokenCommand(transaction.NewContractAddress, transaction.BlockHeight));

            // Get token summary
            var stakingTokenSummary = await _mediator.Send(new RetrieveStakingTokenContractSummaryQuery(transaction.NewContractAddress,
                                                                                                        transaction.BlockHeight,
                                                                                                        includeVault: true,
                                                                                                        includeMiningGovernance: true));

            // Get and/or create vault
            var vault = await _mediator.Send(new CreateVaultCommand(stakingTokenSummary.Vault.GetValueOrDefault(),
                                                                    stakingTokenId, transaction.BlockHeight));

            // Get and/or create mining governance
            var miningGovernanceId = await _mediator.Send(new CreateMiningGovernanceCommand(stakingTokenSummary.MiningGovernance.GetValueOrDefault(),
                                                                                            stakingTokenId, transaction.BlockHeight));

            if (transaction.Id == 0)
            {
                await _mediator.Send(new MakeTransactionCommand(transaction));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failure processing mined token and mining governance deployment.");
        }

        return Unit.Value;
    }
}
