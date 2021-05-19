using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Pools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MarketDeployers;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Markets;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningGovernance;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vault;

namespace Opdex.Platform.Application.EntryHandlers.Transactions
{
    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger _logger;
        private readonly IModelAssembler<Transaction, TransactionDto> _assembler;
        
        public CreateTransactionCommandHandler(IMapper mapper, IMediator mediator,
            ILogger<CreateTransactionCommandHandler> logger, IModelAssembler<Transaction, TransactionDto> assembler)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _assembler = assembler ?? throw new ArgumentNullException(nameof(assembler));
        }
        
        public async Task<bool> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            var transactionDto = await TryGetExistingTransaction(request.TxHash, CancellationToken.None);

            if (transactionDto != null) return true;

            var transaction = await _mediator.Send(new RetrieveCirrusTransactionByHashQuery(request.TxHash), CancellationToken.None);

            if (transaction == null) return false;
            
            var result = await _mediator.Send(new MakeTransactionCommand(transaction), CancellationToken.None);

            if (!result) return false;

            // Todo: These "Process" handlers should persist log as not processed, process log, update log marking it as processed
            foreach (var log in transaction.Logs.OrderBy(l => l.SortOrder))
            {
                bool success = false;
                
                try
                {
                    success = log.LogType switch
                    {
                        TransactionLogType.CreateMarketLog => await _mediator.Send(new ProcessCreateMarketLogCommand(log, transaction.BlockHeight), CancellationToken.None),
                        TransactionLogType.CreateLiquidityPoolLog => await _mediator.Send(new ProcessCreateLiquidityPoolLogCommand(log), CancellationToken.None),
                        TransactionLogType.ChangeMarketOwnerLog => await _mediator.Send(new ProcessChangeMarketOwnerLogCommand(log), CancellationToken.None),
                        TransactionLogType.ChangeMarketPermissionLog =>  await _mediator.Send(new ProcessChangeMarketPermissionLogCommand(log), CancellationToken.None),
                        TransactionLogType.MintLog => await _mediator.Send(new ProcessMintLogCommand(log), CancellationToken.None),
                        TransactionLogType.BurnLog => await _mediator.Send(new ProcessBurnLogCommand(log), CancellationToken.None),
                        TransactionLogType.SwapLog => await _mediator.Send(new ProcessSwapLogCommand(log), CancellationToken.None),
                        TransactionLogType.ReservesLog => await _mediator.Send(new ProcessReservesLogCommand(log), CancellationToken.None),
                        TransactionLogType.ApprovalLog => await _mediator.Send(new ProcessApprovalLogCommand(log), CancellationToken.None),
                        TransactionLogType.TransferLog => await _mediator.Send(new ProcessTransferLogCommand(log, transaction.From, transaction.BlockHeight), CancellationToken.None),
                        TransactionLogType.ChangeMarketLog => await _mediator.Send(new ProcessChangeMarketLogCommand(log), CancellationToken.None),
                        TransactionLogType.StartStakingLog => await _mediator.Send(new ProcessStartStakingLogCommand(log, transaction.BlockHeight), CancellationToken.None),
                        TransactionLogType.CollectStakingRewardsLog => await _mediator.Send(new ProcessCollectStakingRewardsLogCommand(log), CancellationToken.None),
                        TransactionLogType.StopStakingLog => await _mediator.Send(new ProcessStopStakingLogCommand(log, transaction.BlockHeight), CancellationToken.None),
                        TransactionLogType.CreateMiningPoolLog => await _mediator.Send(new ProcessCreateMiningPoolLogCommand(log, transaction.BlockHeight), CancellationToken.None),
                        TransactionLogType.RewardMiningPoolLog => await _mediator.Send(new ProcessRewardMiningPoolLogCommand(log), CancellationToken.None),
                        TransactionLogType.NominationLog => await _mediator.Send(new ProcessNominationLogCommand(log), CancellationToken.None),
                        TransactionLogType.StartMiningLog => await _mediator.Send(new ProcessStartMiningLogCommand(log, transaction.BlockHeight), CancellationToken.None),
                        TransactionLogType.CollectMiningRewardsLog => await _mediator.Send(new ProcessCollectMiningRewardsLogCommand(log), CancellationToken.None),
                        TransactionLogType.StopMiningLog => await _mediator.Send(new ProcessStopMiningLogCommand(log, transaction.BlockHeight), CancellationToken.None),
                        TransactionLogType.EnableMiningLog => await _mediator.Send(new ProcessEnableMiningLogCommand(log), CancellationToken.None),
                        TransactionLogType.DistributionLog => await _mediator.Send(new ProcessDistributionLogCommand(log, transaction.BlockHeight), CancellationToken.None),
                        TransactionLogType.CreateVaultCertificateLog => await _mediator.Send(new ProcessCreateVaultCertificateLogCommand(log, transaction.BlockHeight), CancellationToken.None),
                        TransactionLogType.RevokeVaultCertificateLog => await _mediator.Send(new ProcessRevokeVaultCertificateLogCommand(log), CancellationToken.None),
                        TransactionLogType.RedeemVaultCertificateLog => await _mediator.Send(new ProcessRedeemVaultCertificateLogCommand(log), CancellationToken.None),
                        TransactionLogType.ChangeVaultOwnerLog => await _mediator.Send(new ProcessChangeVaultOwnerLogCommand(log), CancellationToken.None),
                        TransactionLogType.ChangeDeployerOwnerLog => await _mediator.Send(new ProcessChangeDeployerOwnerLogCommand(log), CancellationToken.None),
                        _ => throw new ArgumentOutOfRangeException(nameof(TransactionLogType), "Unknown transaction log type.")
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process transaction log.");
                }

                if (!success)
                {
                    // Handle the transaction, maybe delete all related logs and flag it as errored
                }
            }
            
            // Mark transaction confirming logs all relevant logs were processed


            // Process snapshots the transaction affects should update or create records
            // Consider flagging logs as snapshotProcessed or something else?
            // Maybe process these snapshots once per block after all transactions have been processed for performance.
            // Returning out the Transactions from this query that require snapshots to be processed.
            await _mediator.Send(new ProcessLiquidityPoolSnapshotsByTransactionCommand(transaction.Hash), CancellationToken.None);
            // Todo: Process mining pool snapshots this transaction affects
            // Todo: Process token snapshots this transaction has Transfer logs for
            
            return true;
        }

        private async Task<TransactionDto> TryGetExistingTransaction(string txHash, CancellationToken cancellationToken)
        {
            TransactionDto transactionDto = null;
            
            try
            {
                var transaction = await _mediator.Send(new RetrieveTransactionByHashQuery(txHash), cancellationToken);
                transactionDto = await _assembler.Assemble(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, $"{nameof(Transaction)} with hash {txHash} is not found. Fetching from Cirrus to index.");
            }

            return transactionDto;
        }
    }
}