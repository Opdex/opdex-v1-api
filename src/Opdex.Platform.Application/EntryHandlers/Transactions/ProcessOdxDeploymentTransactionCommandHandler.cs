using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Abstractions.Commands.MiningGovernance;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernance;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.ODX;

namespace Opdex.Platform.Application.EntryHandlers.Transactions
{
    public class ProcessOdxDeploymentTransactionCommandHandler : IRequestHandler<ProcessOdxDeploymentTransactionCommand, Unit>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessOdxDeploymentTransactionCommandHandler> _logger;

        public ProcessOdxDeploymentTransactionCommandHandler(IMediator mediator, ILogger<ProcessOdxDeploymentTransactionCommandHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(ProcessOdxDeploymentTransactionCommand request, CancellationToken cancellationToken)
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
                    var blockTime = blockReceiptDto.Time;
                    var blockMedianTime = blockReceiptDto.MedianTime;
                    var blockCommand = new MakeBlockCommand(blockReceiptDto.Height, blockReceiptDto.Hash, blockTime, blockMedianTime);
                    var blockCreated = await _mediator.Send(blockCommand, CancellationToken.None);
                }

                // Insert ODX
                var odxAddress = transaction.NewContractAddress;
                var odxQuery = new RetrieveTokenByAddressQuery(odxAddress, findOrThrow: false);
                var odx = await _mediator.Send(odxQuery, CancellationToken.None);

                var odxId = odx?.Id ?? 0L;
                if (odx == null)
                {
                    var odxCommand = new MakeTokenCommand(odxAddress);
                    odxId = await _mediator.Send(odxCommand, CancellationToken.None);
                }

                // Get ODX token summary
                var odxTokenSummaryQuery = new RetrieveStakingTokenContractSummaryByAddressQuery(odxAddress);
                var odxTokenSummary = await _mediator.Send(odxTokenSummaryQuery, CancellationToken.None);

                // Get and/or create vault
                var vaultQuery = new RetrieveVaultQuery(findOrThrow: false);
                var vault = await _mediator.Send(vaultQuery, CancellationToken.None);

                if (vault == null)
                {
                    vault = new Vault(odxTokenSummary.Vault, odxId, transaction.From, transaction.BlockHeight, transaction.BlockHeight);
                    var vaultId = await _mediator.Send(new MakeVaultCommand(vault), CancellationToken.None);
                }

                // Get and/or create mining governance
                var miningGovernanceQuery = new RetrieveMiningGovernanceQuery(findOrThrow: false);
                var miningGovernance = await _mediator.Send(miningGovernanceQuery, CancellationToken.None);

                if (miningGovernance == null)
                {
                    // Get governance contract summary
                    var miningGovernanceSummaryQuery = new RetrieveMiningGovernanceContractSummaryByAddressQuery(odxTokenSummary.MiningGovernance);
                    var miningGovernanceSummary = await _mediator.Send(miningGovernanceSummaryQuery, CancellationToken.None);

                    // Create
                    miningGovernance = new MiningGovernance(odxTokenSummary.MiningGovernance, odxId, miningGovernanceSummary.NominationPeriodEnd,
                        miningGovernanceSummary.MiningPoolsFunded, miningGovernanceSummary.MiningPoolReward, transaction.BlockHeight);

                    // Persist
                    var miningGovernanceCommand = new MakeMiningGovernanceCommand(miningGovernance);
                    var miningGovernanceId = await _mediator.Send(miningGovernanceCommand, CancellationToken.None);
                }

                // In hosted environments, transaction would've already been inserted. Local environments need to persist
                if (transaction.Id == 0)
                {
                    await _mediator.Send(new MakeTransactionCommand(transaction), CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failure processing ODX deployment.");
            }

            return Unit.Value;
        }
    }
}
