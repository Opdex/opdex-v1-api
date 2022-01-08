using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.TransactionLogs;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Extensions;

public static class TransactionEventExtensions
{
    public static IEnumerable<TransactionLogType> GetLogTypes(this TransactionEventType eventType)
    {
        return eventType switch
        {
            // Deployers
            TransactionEventType.ClaimPendingDeployerOwnershipEvent => new[] { TransactionLogType.ClaimPendingDeployerOwnershipLog },
            TransactionEventType.SetPendingDeployerOwnershipEvent => new[] { TransactionLogType.SetPendingDeployerOwnershipLog },
            TransactionEventType.CreateMarketEvent => new[] { TransactionLogType.CreateMarketLog },
            // Markets
            TransactionEventType.ClaimPendingMarketOwnershipEvent => new[] { TransactionLogType.ClaimPendingMarketOwnershipLog },
            TransactionEventType.SetPendingMarketOwnershipEvent => new[] { TransactionLogType.SetPendingMarketOwnershipLog },
            TransactionEventType.ChangeMarketPermissionEvent => new[] { TransactionLogType.ChangeMarketPermissionLog },
            TransactionEventType.CreateLiquidityPoolEvent => new[] { TransactionLogType.CreateLiquidityPoolLog },
            // Liquidity Pools
            TransactionEventType.SwapEvent => new[] { TransactionLogType.SwapLog },
            TransactionEventType.AddLiquidityEvent => new[] { TransactionLogType.MintLog },
            TransactionEventType.RemoveLiquidityEvent => new[] { TransactionLogType.BurnLog },
            TransactionEventType.StartStakingEvent => new[] { TransactionLogType.StartStakingLog },
            TransactionEventType.StopStakingEvent => new[] { TransactionLogType.StopStakingLog },
            TransactionEventType.CollectStakingRewardsEvent => new[] { TransactionLogType.CollectStakingRewardsLog },
            // Mining Pools
            TransactionEventType.EnableMiningEvent => new[] { TransactionLogType.EnableMiningLog },
            TransactionEventType.StartMiningEvent => new[] { TransactionLogType.StartMiningLog },
            TransactionEventType.StopMiningEvent => new[] { TransactionLogType.StopMiningLog },
            TransactionEventType.CollectMiningRewardsEvent => new[] { TransactionLogType.CollectMiningRewardsLog },
            // Tokens
            TransactionEventType.TransferEvent => new[] { TransactionLogType.TransferLog },
            TransactionEventType.ApprovalEvent => new[] { TransactionLogType.ApprovalLog },
            TransactionEventType.DistributionEvent => new[] { TransactionLogType.DistributionLog },
            // Mining Governances
            TransactionEventType.NominationEvent => new[] { TransactionLogType.NominationLog },
            TransactionEventType.RewardMiningPoolEvent => new[] { TransactionLogType.RewardMiningPoolLog },
            // Vaults
            TransactionEventType.CreateVaultCertificateEvent => new[] { TransactionLogType.CreateVaultCertificateLog },
            TransactionEventType.RedeemVaultCertificateEvent => new[] { TransactionLogType.RedeemVaultCertificateLog },
            TransactionEventType.RevokeVaultCertificateEvent => new[] { TransactionLogType.RevokeVaultCertificateLog },
            // Vault Governance
            TransactionEventType.CompleteVaultProposalEvent => new[] { TransactionLogType.CompleteVaultProposalLog },
            TransactionEventType.CreateVaultProposalEvent => new[] { TransactionLogType.CreateVaultProposalLog },
            TransactionEventType.VaultProposalPledgeEvent => new[] { TransactionLogType.VaultProposalPledgeLog },
            TransactionEventType.VaultProposalWithdrawPledgeEvent => new[] { TransactionLogType.VaultProposalWithdrawPledgeLog },
            TransactionEventType.VaultProposalVoteEvent => new[] { TransactionLogType.VaultProposalVoteLog },
            TransactionEventType.VaultProposalWithdrawVoteEvent => new[] { TransactionLogType.VaultProposalWithdrawVoteLog },

            _ => throw new ArgumentOutOfRangeException(nameof(eventType), "Invalid transaction event type")
        };
    }
}
