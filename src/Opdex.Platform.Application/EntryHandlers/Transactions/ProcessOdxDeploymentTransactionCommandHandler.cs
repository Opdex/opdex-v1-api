using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Abstractions.Commands.MiningGovernance;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.Commands.Vault;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernance;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain;
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
            var transaction = await TryGetExistingTransaction(request.TxHash, CancellationToken.None);

            if (transaction != null)
            {
                throw new Exception("Transaction Exists");
            }

            // Get transaction from Cirrus
            // Todo: Implement TransactionReceiptSummary Domain model, removing the need to get the follow queries block hash.
            transaction = await _mediator.Send(new RetrieveCirrusTransactionByHashQuery(request.TxHash), CancellationToken.None);

            var transactionBlockHash = await _mediator.Send(new RetrieveCirrusBlockHashByHeightQuery(transaction.BlockHeight), CancellationToken.None);
            
            // Insert Block
            var blockDetails = await _mediator.Send(new RetrieveCirrusBlockByHashQuery(transactionBlockHash), CancellationToken.None);
            
            var blockCommand = new MakeBlockCommand(blockDetails.Height, blockDetails.Hash, blockDetails.Time.FromUnixTimeSeconds(), 
                blockDetails.MedianTime.FromUnixTimeSeconds());
            
            var blockCreated = await _mediator.Send(blockCommand, CancellationToken.None);
            
            // Insert ODX
            var odxAddress = transaction.NewContractAddress;
            
            var odxId = await _mediator.Send(new MakeTokenCommand(odxAddress), CancellationToken.None);
            
            // Get ODX token summary
            var odxTokenSummary = await _mediator.Send(new RetrieveStakingTokenContractSummaryByAddressQuery(odxAddress), CancellationToken.None);
            
            // Insert Vault
            var vaultId = await _mediator.Send(new MakeVaultCommand(new Vault(odxTokenSummary.Vault, odxId, transaction.From, transaction.BlockHeight, transaction.BlockHeight)), CancellationToken.None);
            
            // Insert Governance Contract
            var miningGovernanceRequest = new RetrieveMiningGovernanceContractSummaryByAddressQuery(odxTokenSummary.MiningGovernance);
            
            var miningGovernanceSummary = await _mediator.Send(miningGovernanceRequest, CancellationToken.None);

            var miningGovernance = new MiningGovernance(odxTokenSummary.MiningGovernance, odxId, miningGovernanceSummary.NominationPeriodEnd,
                miningGovernanceSummary.Balance, (int)miningGovernanceSummary.MiningPoolsFunded, miningGovernanceSummary.MiningPoolReward,
                transaction.BlockHeight, transaction.BlockHeight);
                    
            var miningGovernanceId = await _mediator.Send(new MakeMiningGovernanceCommand(miningGovernance), CancellationToken.None);

            // Insert Transaction
            await _mediator.Send(new MakeTransactionCommand(transaction), CancellationToken.None);
            
            return Unit.Value;
        }
        
        private async Task<Transaction> TryGetExistingTransaction(string txHash, CancellationToken cancellationToken)
        {
            Transaction transaction = null;
            
            try
            {
                transaction = await _mediator.Send(new RetrieveTransactionByHashQuery(txHash), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, $"{nameof(Transaction)} with hash {txHash} is not found. Fetching from Cirrus to index.");
            }

            return transaction;
        }
    }
}