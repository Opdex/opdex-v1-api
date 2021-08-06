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
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Governances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MarketDeployers;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Markets;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vaults;

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
            var transaction = await _mediator.Send(transactionQuery);

            // Currently stop here, partially captured transactions cannot be synced again...for now.
            if (transaction != null)
            {
                return true;
            }

            var transactionReceiptQuery = new RetrieveCirrusTransactionByHashQuery(request.TxHash);
            transaction = await _mediator.Send(transactionReceiptQuery);

            if (transaction == null)
            {
                return false;
            }

            // Todo: Only persist if IsOpdexTransaction
            // If any logs are a token, liquidity/mining pool, market, deployer, vault or governance contract we track.

            var transactionId = await _mediator.Send(new MakeTransactionCommand(transaction));
            if (transactionId == 0)
            {
                return false;
            }

            transaction.SetId(transactionId);

            var height = transaction.BlockHeight;
            var sender = transaction.From;

            foreach (var log in transaction.Logs.OrderBy(l => l.SortOrder))
            {
                var success = false;

                try
                {
                    success = log.LogType switch
                    {
                        // Deployers
                        TransactionLogType.CreateMarketLog => await _mediator.Send(new ProcessCreateMarketLogCommand(log, sender, height)),
                        TransactionLogType.ClaimPendingDeployerOwnershipLog => await _mediator.Send(new ProcessClaimPendingDeployerOwnershipLogCommand(log, sender, height)),
                        TransactionLogType.SetPendingDeployerOwnershipLog => await _mediator.Send(new ProcessSetPendingDeployerOwnershipLogCommand(log, sender, height)),

                        // Markets
                        TransactionLogType.CreateLiquidityPoolLog => await _mediator.Send(new ProcessCreateLiquidityPoolLogCommand(log, sender, height)),
                        TransactionLogType.ClaimPendingMarketOwnershipLog => await _mediator.Send(new ProcessClaimPendingMarketOwnershipLogCommand(log, sender, height)),
                        TransactionLogType.SetPendingMarketOwnershipLog => await _mediator.Send(new ProcessSetPendingMarketOwnershipLogCommand(log, sender, height)),
                        TransactionLogType.ChangeMarketPermissionLog => await _mediator.Send(new ProcessChangeMarketPermissionLogCommand(log, sender, height)),

                        // Liquidity Pools
                        TransactionLogType.MintLog => await _mediator.Send(new ProcessMintLogCommand(log, sender, height)),
                        TransactionLogType.BurnLog => await _mediator.Send(new ProcessBurnLogCommand(log, sender, height)),
                        TransactionLogType.SwapLog => await _mediator.Send(new ProcessSwapLogCommand(log, sender, height)),
                        TransactionLogType.ReservesLog => await _mediator.Send(new ProcessReservesLogCommand(log, sender, height)),
                        TransactionLogType.StartStakingLog => await _mediator.Send(new ProcessStakeLogCommand(log, sender, height)),
                        TransactionLogType.StopStakingLog => await _mediator.Send(new ProcessStakeLogCommand(log, sender, height)),
                        TransactionLogType.CollectStakingRewardsLog => await _mediator.Send(new ProcessCollectStakingRewardsLogCommand(log, sender, height)),

                        // Mining Pools
                        TransactionLogType.StartMiningLog => await _mediator.Send(new ProcessMineLogCommand(log, sender, height)),
                        TransactionLogType.StopMiningLog => await _mediator.Send(new ProcessMineLogCommand(log, sender, height)),
                        TransactionLogType.CollectMiningRewardsLog => await _mediator.Send(new ProcessCollectMiningRewardsLogCommand(log, sender, height)),
                        TransactionLogType.EnableMiningLog => await _mediator.Send(new ProcessEnableMiningLogCommand(log, sender, height)),

                        // Tokens
                        TransactionLogType.ApprovalLog => await _mediator.Send(new ProcessApprovalLogCommand(log, sender, height)),
                        TransactionLogType.TransferLog => await _mediator.Send(new ProcessTransferLogCommand(log, sender, height)),
                        TransactionLogType.DistributionLog => await _mediator.Send(new ProcessDistributionLogCommand(log, sender, height)),

                        // Governances
                        TransactionLogType.RewardMiningPoolLog => await _mediator.Send(new ProcessRewardMiningPoolLogCommand(log, sender, height)),
                        TransactionLogType.NominationLog => await _mediator.Send(new ProcessNominationLogCommand(log, sender, height)),

                        // Vault
                        TransactionLogType.CreateVaultCertificateLog => await _mediator.Send(new ProcessCreateVaultCertificateLogCommand(log, sender, height)),
                        TransactionLogType.RevokeVaultCertificateLog => await _mediator.Send(new ProcessRevokeVaultCertificateLogCommand(log, sender, height)),
                        TransactionLogType.RedeemVaultCertificateLog => await _mediator.Send(new ProcessRedeemVaultCertificateLogCommand(log, sender, height)),
                        TransactionLogType.ClaimPendingVaultOwnershipLog => await _mediator.Send(new ProcessClaimPendingVaultOwnershipLogCommand(log, sender, height)),
                        TransactionLogType.SetPendingVaultOwnershipLog => await _mediator.Send(new ProcessSetPendingVaultOwnershipLogCommand(log, sender, height)),

                        // Else
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
            await _mediator.Send(new ProcessLiquidityPoolSnapshotsByTransactionCommand(transaction));
            // Todo: Process token snapshots this transaction has Transfer logs for

            return true;
        }
    }
}
