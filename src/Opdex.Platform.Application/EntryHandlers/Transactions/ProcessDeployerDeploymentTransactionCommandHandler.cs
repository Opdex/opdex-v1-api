using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Abstractions.Commands.Deployers;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs;
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

            var log = transaction.LogsOfType<CreateMarketLog>(TransactionLogType.CreateMarketLog).Single();
            
            try
            {
                var odx = await _mediator.Send(new RetrieveTokenByAddressQuery(log.StakingToken), CancellationToken.None);

                var deployer = new Deployer(log.Contract, log.Owner, transaction.BlockHeight);
                
                var deployerId = await _mediator.Send(new MakeDeployerCommand(deployer), CancellationToken.None);

                var market = new Market(log.Market, deployerId, odx.Id, transaction.From, log.AuthPoolCreators, log.AuthProviders, log.AuthTraders, 
                    log.Fee, transaction.BlockHeight);
                
                var marketId = await _mediator.Send(new MakeMarketCommand(market), CancellationToken.None);
                
                // Insert Transaction
                await _mediator.Send(new MakeTransactionCommand(transaction), CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failure processing deployer deployment.");
            }

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
                _logger.LogInformation(ex, $"{nameof(Transaction)} with hash {txHash} is not found.");
            }

            return transaction;
        }
    }
}