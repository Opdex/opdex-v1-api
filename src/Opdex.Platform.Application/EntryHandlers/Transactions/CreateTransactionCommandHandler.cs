using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
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
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public CreateTransactionCommandHandler(IMediator mediator, ILogger<CreateTransactionCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            var transactionQuery = new RetrieveTransactionByHashQuery(request.TxHash, findOrThrow: false);
            var transaction = await _mediator.Send(transactionQuery, CancellationToken.None);

            // Currently stop here, partially captured transactions cannot be synced again...for now.
            if (transaction != null)
            {
                return true;
            }

            var transactionReceiptQuery = new RetrieveCirrusTransactionByHashQuery(request.TxHash);
            transaction = await _mediator.Send(transactionReceiptQuery, CancellationToken.None);

            if (transaction == null)
            {
                return false;
            }

            // Todo: Only persist if IsOpdexTransaction

            var transactionId = await _mediator.Send(new MakeTransactionCommand(transaction), CancellationToken.None);
            if (transactionId == 0)
            {
                return false;
            }

            transaction.SetId(transactionId);

            var height = transaction.BlockHeight;
            var sender = transaction.From;

            foreach (var log in transaction.Logs.OrderByDescending(l => l.SortOrder))
            {
                var success = false;

                try
                {
                    success = log.LogType switch
                    {
                        TransactionLogType.CreateMarketLog => await _mediator.Send(new ProcessCreateMarketLogCommand(log, sender, height), CancellationToken.None),
                        TransactionLogType.CreateLiquidityPoolLog => await _mediator.Send(new ProcessCreateLiquidityPoolLogCommand(log, sender, height), CancellationToken.None),
                        TransactionLogType.ChangeMarketOwnerLog => await _mediator.Send(new ProcessChangeMarketOwnerLogCommand(log, sender, height), CancellationToken.None),
                        TransactionLogType.ChangeMarketPermissionLog => await _mediator.Send(new ProcessChangeMarketPermissionLogCommand(log, sender, height), CancellationToken.None),
                        TransactionLogType.MintLog => await _mediator.Send(new ProcessMintLogCommand(log, sender, height), CancellationToken.None),
                        TransactionLogType.BurnLog => await _mediator.Send(new ProcessBurnLogCommand(log, sender, height), CancellationToken.None),
                        TransactionLogType.SwapLog => await _mediator.Send(new ProcessSwapLogCommand(log, sender, height), CancellationToken.None),
                        TransactionLogType.ReservesLog => await _mediator.Send(new ProcessReservesLogCommand(log, sender, height), CancellationToken.None),
                        TransactionLogType.ApprovalLog => await _mediator.Send(new ProcessApprovalLogCommand(log, sender, height), CancellationToken.None),
                        TransactionLogType.TransferLog => await _mediator.Send(new ProcessTransferLogCommand(log, sender, height), CancellationToken.None),
                        TransactionLogType.ChangeMarketLog => await _mediator.Send(new ProcessChangeMarketLogCommand(log, sender, height), CancellationToken.None),
                        TransactionLogType.StakeLog => await _mediator.Send(new ProcessStakeLogCommand(log, sender, height), CancellationToken.None),
                        TransactionLogType.CollectStakingRewardsLog => await _mediator.Send(new ProcessCollectStakingRewardsLogCommand(log, sender, height), CancellationToken.None),
                        TransactionLogType.RewardMiningPoolLog => await _mediator.Send(new ProcessRewardMiningPoolLogCommand(log, sender, height), CancellationToken.None),
                        TransactionLogType.NominationLog => await _mediator.Send(new ProcessNominationLogCommand(log, sender, height), CancellationToken.None),
                        TransactionLogType.MineLog => await _mediator.Send(new ProcessMineLogCommand(log, sender, height), CancellationToken.None),
                        TransactionLogType.CollectMiningRewardsLog => await _mediator.Send(new ProcessCollectMiningRewardsLogCommand(log, sender, height), CancellationToken.None),
                        TransactionLogType.EnableMiningLog => await _mediator.Send(new ProcessEnableMiningLogCommand(log, sender, height), CancellationToken.None),
                        TransactionLogType.DistributionLog => await _mediator.Send(new ProcessDistributionLogCommand(log, sender, height), CancellationToken.None),
                        TransactionLogType.CreateVaultCertificateLog => await _mediator.Send(new ProcessCreateVaultCertificateLogCommand(log, sender, height), CancellationToken.None),
                        TransactionLogType.RevokeVaultCertificateLog => await _mediator.Send(new ProcessRevokeVaultCertificateLogCommand(log, sender, height), CancellationToken.None),
                        TransactionLogType.RedeemVaultCertificateLog => await _mediator.Send(new ProcessRedeemVaultCertificateLogCommand(log, sender, height), CancellationToken.None),
                        TransactionLogType.ChangeVaultOwnerLog => await _mediator.Send(new ProcessChangeVaultOwnerLogCommand(log, sender, height), CancellationToken.None),
                        TransactionLogType.ChangeDeployerOwnerLog => await _mediator.Send(new ProcessChangeDeployerOwnerLogCommand(log, sender, height), CancellationToken.None),
                        _ => throw new ArgumentOutOfRangeException(nameof(TransactionLogType), "Unknown transaction log type.")
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process transaction log.");
                }

                if (!success)
                {
                    _logger.LogError($"Failed to persist transaction log type {log.LogType}.");
                }
            }

            // Mark transaction confirming logs all relevant logs were processed


            // Process snapshots the transaction affects should update or create records
            // Consider flagging logs as snapshotProcessed or something else?
            // Maybe process these snapshots once per block after all transactions have been processed for performance.
            // Returning out the Transactions from this query that require snapshots to be processed.
            await _mediator.Send(new ProcessLiquidityPoolSnapshotsByTransactionCommand(transaction), CancellationToken.None);
            // Todo: Process mining pool snapshots this transaction affects
            // Todo: Process token snapshots this transaction has Transfer logs for

            return true;
        }
    }
}