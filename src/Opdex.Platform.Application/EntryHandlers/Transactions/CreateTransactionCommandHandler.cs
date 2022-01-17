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
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningGovernances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MarketDeployers;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Markets;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.SignalR.Commands;
using System.Collections.Generic;

namespace Opdex.Platform.Application.EntryHandlers.Transactions;

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
        var tx = await _mediator.Send(new RetrieveTransactionByHashQuery(request.TxHash, findOrThrow: false));

        // If the transaction already exists, skip it
        if (tx != null) return true;

        // Get the transaction from Cirrus
        tx = await _mediator.Send(new RetrieveCirrusTransactionByHashQuery(request.TxHash));

        // Can't process transactions we don't find
        if (tx == null) return false;

        // No logs w/ success and empty newContractAddress is Ineligible
        if (tx.Logs.Count == 0 && tx.Success && tx.NewContractAddress == Address.Empty) return false;

        // If all logs are transfer logs, process balance updates of tokens we know about and exit
        if (tx.Logs.Count > 0 && tx.Logs.All(log => log.LogType == TransactionLogType.TransferLog))
        {
            await Task.WhenAll(tx.Logs.Select(log => _mediator.Send(new ProcessTransferLogCommand(log, tx.From, tx.BlockHeight))));
            return false;
        }

        if (tx.Success)
        {
            // Validate and process all available logs - It only takes a single success to index the entire transaction
            var isValidTx = await ProcessTransactionLogs(tx);
            if (!isValidTx) return false;
        }
        else
        {
            var isToOpdexContract = await IsToOpdexContract(tx.To);
            if (!isToOpdexContract) return false;
        }

        // Persist the transaction if logs were processed or its a failed Opdex related transaction
        var txId = await _mediator.Send(new MakeTransactionCommand(tx));
        if (txId == 0) return false;

        // Only attempt to process snapshots this transaction affects on successful transactions
        if (tx.Success)
        {
            await _mediator.Send(new ProcessLiquidityPoolSnapshotsByTransactionCommand(tx));
        }

        // Notify the user we found a pending transaction of theirs
        await _mediator.Send(new NotifyUserOfMinedTransactionCommand(tx.From, tx.Hash));

        return true;
    }

    /// <summary>
    /// Queries each individual contract type we store looking for a match.
    /// </summary>
    /// <param name="to">The address the transaction was sent to.</param>
    /// <returns>Boolean as success</returns>
    private async Task<bool> IsToOpdexContract(Address to)
    {
        const bool findOrThrow = false;

        // Order the following checks by likelihood
        var router = await _mediator.Send(new RetrieveMarketRouterByAddressQuery(to, findOrThrow));
        if (router != null) return true;

        // Also takes care of liquidity pool check by looking for the equivalent OLPT token
        var token = await _mediator.Send(new RetrieveTokenByAddressQuery(to, findOrThrow));
        if (token != null) return true;

        var miningPool = await _mediator.Send(new RetrieveMiningPoolByAddressQuery(to, findOrThrow));
        if (miningPool != null) return true;

        var vault = await _mediator.Send(new RetrieveVaultByAddressQuery(to, findOrThrow));
        if (vault != null) return true;

        var market = await _mediator.Send(new RetrieveMarketByAddressQuery(to, findOrThrow));
        if (market != null) return true;

        var miningGovernance = await _mediator.Send(new RetrieveMiningGovernanceByAddressQuery(to, findOrThrow));
        if (miningGovernance != null) return true;

        var deployer = await _mediator.Send(new RetrieveDeployerByAddressQuery(to, findOrThrow));
        return deployer != null;
    }

    /// <summary>
    /// Loop and attempt to process each of the available logs in the transaction. Each log validates itself and
    /// a single successful processed log allows for indexing of the entire transaction.
    /// </summary>
    /// <param name="tx">The transaction being indexed.</param>
    /// <returns>Boolean value if at least a single log is processed.</returns>
    private async Task<bool> ProcessTransactionLogs(Transaction tx)
    {
        var shouldIndex = false;

        // Process all in order, in certain scenarios such as CreateMarket transactions, it matters.
        foreach (var log in tx.Logs.OrderBy(l => l.SortOrder))
        {
            try
            {
                var processedLog = log.LogType switch
                {
                    // Deployers
                    TransactionLogType.CreateMarketLog => await _mediator.Send(new ProcessCreateMarketLogCommand(log, tx.From, tx.BlockHeight)),
                    TransactionLogType.ClaimPendingDeployerOwnershipLog => await _mediator.Send(new ProcessClaimPendingDeployerOwnershipLogCommand(log, tx.From, tx.BlockHeight)),
                    TransactionLogType.SetPendingDeployerOwnershipLog => await _mediator.Send(new ProcessSetPendingDeployerOwnershipLogCommand(log, tx.From, tx.BlockHeight)),
                    // Markets
                    TransactionLogType.CreateLiquidityPoolLog => await _mediator.Send(new ProcessCreateLiquidityPoolLogCommand(log, tx.From, tx.BlockHeight)),
                    TransactionLogType.ClaimPendingMarketOwnershipLog => await _mediator.Send(new ProcessClaimPendingMarketOwnershipLogCommand(log, tx.From, tx.BlockHeight)),
                    TransactionLogType.SetPendingMarketOwnershipLog => await _mediator.Send(new ProcessSetPendingMarketOwnershipLogCommand(log, tx.From, tx.BlockHeight)),
                    TransactionLogType.ChangeMarketPermissionLog => await _mediator.Send(new ProcessChangeMarketPermissionLogCommand(log, tx.From, tx.BlockHeight)),
                    // Liquidity Pools
                    TransactionLogType.MintLog => await _mediator.Send(new ProcessMintLogCommand(log, tx.From, tx.BlockHeight)),
                    TransactionLogType.BurnLog => await _mediator.Send(new ProcessBurnLogCommand(log, tx.From, tx.BlockHeight)),
                    TransactionLogType.SwapLog => await _mediator.Send(new ProcessSwapLogCommand(log, tx.From, tx.BlockHeight)),
                    TransactionLogType.ReservesLog => await _mediator.Send(new ProcessReservesLogCommand(log, tx.From, tx.BlockHeight)),
                    TransactionLogType.StartStakingLog => await _mediator.Send(new ProcessStakeLogCommand(log, tx.From, tx.BlockHeight)),
                    TransactionLogType.StopStakingLog => await _mediator.Send(new ProcessStakeLogCommand(log, tx.From, tx.BlockHeight)),
                    TransactionLogType.CollectStakingRewardsLog => await _mediator.Send(new ProcessCollectStakingRewardsLogCommand(log, tx.From, tx.BlockHeight)),
                    // Mining Pools
                    TransactionLogType.StartMiningLog => await _mediator.Send(new ProcessMineLogCommand(log, tx.From, tx.BlockHeight)),
                    TransactionLogType.StopMiningLog => await _mediator.Send(new ProcessMineLogCommand(log, tx.From, tx.BlockHeight)),
                    TransactionLogType.CollectMiningRewardsLog => await _mediator.Send(new ProcessCollectMiningRewardsLogCommand(log, tx.From, tx.BlockHeight)),
                    TransactionLogType.EnableMiningLog => await _mediator.Send(new ProcessEnableMiningLogCommand(log, tx.From, tx.BlockHeight)),
                    // Tokens
                    TransactionLogType.ApprovalLog => await _mediator.Send(new ProcessApprovalLogCommand(log, tx.From, tx.BlockHeight)),
                    TransactionLogType.TransferLog => await _mediator.Send(new ProcessTransferLogCommand(log, tx.From, tx.BlockHeight)),
                    TransactionLogType.DistributionLog => await _mediator.Send(new ProcessDistributionLogCommand(log, tx.From, tx.BlockHeight)),
                    // Mining Governances
                    TransactionLogType.RewardMiningPoolLog => await _mediator.Send(new ProcessRewardMiningPoolLogCommand(log, tx.From, tx.BlockHeight)),
                    TransactionLogType.NominationLog => await _mediator.Send(new ProcessNominationLogCommand(log, tx.From, tx.BlockHeight)),
                    // Vault
                    TransactionLogType.CreateVaultCertificateLog => await _mediator.Send(new ProcessCreateVaultCertificateLogCommand(log, tx.From, tx.BlockHeight)),
                    TransactionLogType.RevokeVaultCertificateLog => await _mediator.Send(new ProcessRevokeVaultCertificateLogCommand(log, tx.From, tx.BlockHeight)),
                    TransactionLogType.RedeemVaultCertificateLog => await _mediator.Send(new ProcessRedeemVaultCertificateLogCommand(log, tx.From, tx.BlockHeight)),
                    // Vault
                    TransactionLogType.CompleteVaultProposalLog => await _mediator.Send(new ProcessCompleteVaultProposalLogCommand(log, tx.From, tx.BlockHeight)),
                    TransactionLogType.CreateVaultProposalLog => await _mediator.Send(new ProcessCreateVaultProposalLogCommand(log, tx.From, tx.BlockHeight)),
                    TransactionLogType.VaultProposalPledgeLog => await _mediator.Send(new ProcessVaultProposalPledgeLogCommand(log, tx.From, tx.BlockHeight)),
                    TransactionLogType.VaultProposalWithdrawPledgeLog => await _mediator.Send(new ProcessVaultProposalWithdrawPledgeLogCommand(log, tx.From, tx.BlockHeight)),
                    TransactionLogType.VaultProposalVoteLog => await _mediator.Send(new ProcessVaultProposalVoteLogCommand(log, tx.From, tx.BlockHeight)),
                    TransactionLogType.VaultProposalWithdrawVoteLog => await _mediator.Send(new ProcessVaultProposalWithdrawVoteLogCommand(log, tx.From, tx.BlockHeight)),

                    // Else
                    _ => throw new ArgumentOutOfRangeException(nameof(TransactionLogType), "Unknown transaction log type.")
                };

                if (processedLog)
                {
                    shouldIndex = true;
                }
                else
                {
                    using (_logger.BeginScope(new Dictionary<string, object>
                           {
                               ["Contract"] = log.Contract,
                               ["LogType"] = log.LogType,
                               ["TransactionHash"] = tx.Hash
                           }))
                    {
                        _logger.LogWarning("Transaction log unsuccessfully processed");
                    }
                }
            }
            catch (Exception ex)
            {
                using (_logger.BeginScope(new Dictionary<string, object>
                       {
                           ["Contract"] = log.Contract,
                           ["LogType"] = log.LogType,
                           ["TransactionHash"] = tx.Hash
                       }))
                {
                    _logger.LogError(ex, "Unexpected error processing transaction log");
                }
            }
        }

        return shouldIndex;
    }
}
