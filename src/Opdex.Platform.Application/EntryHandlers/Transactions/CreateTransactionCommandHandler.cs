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
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs;
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
                bool success;
                
                try
                {
                    success = log.LogType switch
                    {
                        TransactionLogType.MarketCreatedLog => await _mediator.Send(new ProcessMarketCreatedLogCommand(log, transaction.From), CancellationToken.None),
                        TransactionLogType.LiquidityPoolCreatedLog => await _mediator.Send(new ProcessLiquidityPoolCreatedLogCommand(log), CancellationToken.None),
                        TransactionLogType.MarketOwnerChangeLog => await _mediator.Send(new ProcessMarketOwnerChangeLogCommand(log), CancellationToken.None),
                        TransactionLogType.PermissionsChangeLog =>  await _mediator.Send(new ProcessPermissionsChangeLogCommand(log), CancellationToken.None),
                        TransactionLogType.MintLog => await _mediator.Send(new ProcessMintLogCommand(log), CancellationToken.None),
                        TransactionLogType.BurnLog => await _mediator.Send(new ProcessBurnLogCommand(log), CancellationToken.None),
                        TransactionLogType.SwapLog => await _mediator.Send(new ProcessSwapLogCommand(log), CancellationToken.None),
                        TransactionLogType.ReservesLog => await _mediator.Send(new ProcessReservesLogCommand(log), CancellationToken.None),
                        TransactionLogType.ApprovalLog => await _mediator.Send(new ProcessApprovalLogCommand(log), CancellationToken.None),
                        TransactionLogType.TransferLog => await _mediator.Send(new ProcessTransferLogCommand(log), CancellationToken.None),
                        TransactionLogType.MarketChangeLog => await _mediator.Send(new ProcessMarketChangeLogCommand(log), CancellationToken.None),
                        TransactionLogType.StartStakingLog => await _mediator.Send(new ProcessStartStakingLogCommand(log), CancellationToken.None),
                        TransactionLogType.CollectStakingRewardsLog => await _mediator.Send(new ProcessCollectStakingRewardsLogCommand(log), CancellationToken.None),
                        TransactionLogType.StopStakingLog => await _mediator.Send(new ProcessStopStakingLogCommand(log), CancellationToken.None),
                        TransactionLogType.MiningPoolCreatedLog => await _mediator.Send(new ProcessMiningPoolCreatedLogCommand(log), CancellationToken.None),
                        TransactionLogType.RewardMiningPoolLog => await _mediator.Send(new ProcessRewardMiningPoolLogCommand(log), CancellationToken.None),
                        TransactionLogType.NominationLog => await _mediator.Send(new ProcessNominationLogCommand(log), CancellationToken.None),
                        TransactionLogType.StartMiningLog => await _mediator.Send(new ProcessStartMiningLogCommand(log), CancellationToken.None),
                        TransactionLogType.CollectMiningRewardsLog => await _mediator.Send(new ProcessCollectMiningRewardsLogCommand(log), CancellationToken.None),
                        TransactionLogType.StopMiningLog => await _mediator.Send(new ProcessStopMiningLogCommand(log), CancellationToken.None),
                        TransactionLogType.MiningPoolRewardedLog => await _mediator.Send(new ProcessMiningPoolRewardedLogCommand(log), CancellationToken.None),
                        TransactionLogType.DistributionLog => await _mediator.Send(new ProcessDistributionLogCommand(log, transaction.BlockHeight), CancellationToken.None),
                        TransactionLogType.VaultCertificateCreatedLog => await _mediator.Send(new ProcessVaultCertificateCreatedLogCommand(log), CancellationToken.None),
                        TransactionLogType.VaultCertificateUpdatedLog => await _mediator.Send(new ProcessVaultCertificateUpdatedLogCommand(log), CancellationToken.None),
                        TransactionLogType.VaultCertificateRedeemedLog => await _mediator.Send(new ProcessVaultCertificateRedeemedLogCommand(log), CancellationToken.None),
                        TransactionLogType.VaultOwnerChangeLog => await _mediator.Send(new ProcessVaultOwnerChangeLogCommand(log), CancellationToken.None)
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process transaction log.");
                    
                    success = false;
                }

                if (!success)
                {
                    throw new Exception("");
                    // Handle the transaction, maybe delete all related logs and flag it as errored
                }
                
                // Mark transaction confirming logs all relevant logs were processed
            }

            // Maybe do this per block rather than per transaction?
            // Process snapshots the transaction affects should update or create records
            // Consider flagging logs as snapshotProcessed or something else?
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