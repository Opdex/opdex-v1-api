using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Governances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MarketDeployers;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Markets;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vaults;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Infrastructure.Abstractions.Clients.SignalR.Commands;
using System.Collections.Generic;

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
            var transaction = await _mediator.Send(new RetrieveTransactionByHashQuery(request.TxHash, findOrThrow: false));

            // If the transaction already exists, skip it
            if (transaction != null) return true;

            // Get the transaction from Cirrus
            transaction = await _mediator.Send(new RetrieveCirrusTransactionByHashQuery(request.TxHash));

            // Can't process transactions we don't find
            if (transaction == null) return false;

            // Initial validation based on logs alone.
            transaction.ValidateLogTypeEligibility();

            // Index everything for right now.
            // switch (transaction.EligibilityStatus)
            // {
            //     // Gracefully skip transactions we don't care about.
            //     case TransactionEligibilityType.Ineligible:
            //         return true;
            //     case TransactionEligibilityType.PendingContractValidation:
            //         // Todo: Validate log contracts
            //         // Maybe process all logs, to do the validation and persist them with the transaction after.
            //         break;
            //     case TransactionEligibilityType.PartiallyEligible:
            //         // Todo: Act on whitelisted known log types such as transfer or approval logs
            //
            //         // Exit gracefully after
            //         return true;
            // }

            // Persist the transaction without logs
            var transactionId = await _mediator.Send(new MakeTransactionCommand(transaction));
            if (transactionId == 0) return false;

            transaction.SetId(transactionId);

            foreach (var log in transaction.Logs.OrderBy(l => l.SortOrder))
            {
                var success = false;

                try
                {
                    success = log.LogType switch
                    {
                        // Deployers
                        TransactionLogType.CreateMarketLog =>
                            await _mediator.Send(new ProcessCreateMarketLogCommand(log, transaction.From, transaction.BlockHeight)),
                        TransactionLogType.ClaimPendingDeployerOwnershipLog =>
                            await _mediator.Send(new ProcessClaimPendingDeployerOwnershipLogCommand(log, transaction.From, transaction.BlockHeight)),
                        TransactionLogType.SetPendingDeployerOwnershipLog =>
                            await _mediator.Send(new ProcessSetPendingDeployerOwnershipLogCommand(log, transaction.From, transaction.BlockHeight)),

                        // Markets
                        TransactionLogType.CreateLiquidityPoolLog =>
                            await _mediator.Send(new ProcessCreateLiquidityPoolLogCommand(log, transaction.From, transaction.BlockHeight)),
                        TransactionLogType.ClaimPendingMarketOwnershipLog =>
                            await _mediator.Send(new ProcessClaimPendingMarketOwnershipLogCommand(log, transaction.From, transaction.BlockHeight)),
                        TransactionLogType.SetPendingMarketOwnershipLog =>
                            await _mediator.Send(new ProcessSetPendingMarketOwnershipLogCommand(log, transaction.From, transaction.BlockHeight)),
                        TransactionLogType.ChangeMarketPermissionLog =>
                            await _mediator.Send(new ProcessChangeMarketPermissionLogCommand(log, transaction.From, transaction.BlockHeight)),

                        // Liquidity Pools
                        TransactionLogType.MintLog =>
                            await _mediator.Send(new ProcessMintLogCommand(log, transaction.From, transaction.BlockHeight)),
                        TransactionLogType.BurnLog =>
                            await _mediator.Send(new ProcessBurnLogCommand(log, transaction.From, transaction.BlockHeight)),
                        TransactionLogType.SwapLog =>
                            await _mediator.Send(new ProcessSwapLogCommand(log, transaction.From, transaction.BlockHeight)),
                        TransactionLogType.ReservesLog =>
                            await _mediator.Send(new ProcessReservesLogCommand(log, transaction.From, transaction.BlockHeight)),
                        TransactionLogType.StartStakingLog =>
                            await _mediator.Send(new ProcessStakeLogCommand(log, transaction.From, transaction.BlockHeight)),
                        TransactionLogType.StopStakingLog =>
                            await _mediator.Send(new ProcessStakeLogCommand(log, transaction.From, transaction.BlockHeight)),
                        TransactionLogType.CollectStakingRewardsLog =>
                            await _mediator.Send(new ProcessCollectStakingRewardsLogCommand(log, transaction.From, transaction.BlockHeight)),

                        // Mining Pools
                        TransactionLogType.StartMiningLog =>
                            await _mediator.Send(new ProcessMineLogCommand(log, transaction.From, transaction.BlockHeight)),
                        TransactionLogType.StopMiningLog =>
                            await _mediator.Send(new ProcessMineLogCommand(log, transaction.From, transaction.BlockHeight)),
                        TransactionLogType.CollectMiningRewardsLog =>
                            await _mediator.Send(new ProcessCollectMiningRewardsLogCommand(log, transaction.From, transaction.BlockHeight)),
                        TransactionLogType.EnableMiningLog =>
                            await _mediator.Send(new ProcessEnableMiningLogCommand(log, transaction.From, transaction.BlockHeight)),

                        // Tokens
                        TransactionLogType.ApprovalLog =>
                            await _mediator.Send(new ProcessApprovalLogCommand(log, transaction.From, transaction.BlockHeight)),
                        TransactionLogType.TransferLog =>
                            await _mediator.Send(new ProcessTransferLogCommand(log, transaction.From, transaction.BlockHeight)),
                        TransactionLogType.DistributionLog =>
                            await _mediator.Send(new ProcessDistributionLogCommand(log, transaction.From, transaction.BlockHeight)),

                        // Governances
                        TransactionLogType.RewardMiningPoolLog =>
                            await _mediator.Send(new ProcessRewardMiningPoolLogCommand(log, transaction.From, transaction.BlockHeight)),
                        TransactionLogType.NominationLog =>
                            await _mediator.Send(new ProcessNominationLogCommand(log, transaction.From, transaction.BlockHeight)),

                        // Vault
                        TransactionLogType.CreateVaultCertificateLog =>
                            await _mediator.Send(new ProcessCreateVaultCertificateLogCommand(log, transaction.From, transaction.BlockHeight)),
                        TransactionLogType.RevokeVaultCertificateLog =>
                            await _mediator.Send(new ProcessRevokeVaultCertificateLogCommand(log, transaction.From, transaction.BlockHeight)),
                        TransactionLogType.RedeemVaultCertificateLog =>
                            await _mediator.Send(new ProcessRedeemVaultCertificateLogCommand(log, transaction.From, transaction.BlockHeight)),
                        TransactionLogType.ClaimPendingVaultOwnershipLog =>
                            await _mediator.Send(new ProcessClaimPendingVaultOwnershipLogCommand(log, transaction.From, transaction.BlockHeight)),
                        TransactionLogType.SetPendingVaultOwnershipLog =>
                            await _mediator.Send(new ProcessSetPendingVaultOwnershipLogCommand(log, transaction.From, transaction.BlockHeight)),

                        // Else
                        _ => throw new ArgumentOutOfRangeException(nameof(TransactionLogType), "Unknown transaction log type.")
                    };
                }
                catch (Exception ex)
                {
                    using (_logger.BeginScope(new Dictionary<string, object>{
                        ["Contract"] = log.Contract,
                        ["LogType"] = log.LogType,
                        ["TransactionHash"] = transaction.Hash
                    }))
                    {
                        _logger.LogError(ex, "Unexpected error processing transaction log");
                    }
                }

                if (success)
                {
                    continue;
                }

                using (_logger.BeginScope(new Dictionary<string, object>{
                    ["Contract"] = log.Contract,
                    ["LogType"] = log.LogType,
                    ["TransactionHash"] = transaction.Hash
                }))
                {
                    _logger.LogWarning("Transaction log unsuccessfully processed");
                }
            }

            await _mediator.Send(new ProcessLiquidityPoolSnapshotsByTransactionCommand(transaction));

            await _mediator.Send(new NotifyUserOfMinedTransactionCommand(transaction.From, transaction.Hash));

            return true;
        }
    }
}
