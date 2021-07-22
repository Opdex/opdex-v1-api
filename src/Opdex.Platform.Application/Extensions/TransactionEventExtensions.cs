using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Domain.Models.TransactionLogs;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Extensions
{
    public static class TransactionEventExtensions
    {
        public static IEnumerable<TransactionLogType> GetLogTypes(this TransactionEventType eventType)
        {
            return eventType switch
            {
                // Deployers
                TransactionEventType.ChangeDeployerOwnerEvent => new[] {TransactionLogType.ChangeDeployerOwnerLog},
                TransactionEventType.CreateMarketEvent => new[] {TransactionLogType.CreateMarketLog},
                // Markets
                TransactionEventType.ChangeMarketOwnerEvent => new[] {TransactionLogType.ChangeDeployerOwnerLog},
                TransactionEventType.ChangeMarketPermissionEvent => new[] {TransactionLogType.ChangeDeployerOwnerLog},
                TransactionEventType.CreateLiquidityPoolEvent => new[] {TransactionLogType.ChangeDeployerOwnerLog},
                // Liquidity Pools
                TransactionEventType.SwapEvent => new[] {TransactionLogType.SwapLog},
                TransactionEventType.ProvideEvent => new[] {TransactionLogType.MineLog, TransactionLogType.BurnLog},
                TransactionEventType.StakeEvent => new[] {TransactionLogType.StakeLog},
                TransactionEventType.CollectStakingRewardsEvent => new[] {TransactionLogType.CollectStakingRewardsLog},
                // Mining Pools
                TransactionEventType.EnableMiningEvent => new[] {TransactionLogType.EnableMiningLog},
                TransactionEventType.MineEvent => new[] {TransactionLogType.MineLog},
                TransactionEventType.CollectMiningRewardsEvent => new[] {TransactionLogType.CollectMiningRewardsLog},
                // Tokens
                TransactionEventType.TransferEvent => new[] {TransactionLogType.TransferLog},
                TransactionEventType.ApprovalEvent => new[] {TransactionLogType.ApprovalLog},
                TransactionEventType.DistributionEvent => new[] {TransactionLogType.DistributionLog},
                // Governances
                TransactionEventType.NominationEvent => new[] {TransactionLogType.NominationLog},
                TransactionEventType.RewardMiningPoolEvent => new[] {TransactionLogType.RewardMiningPoolLog},
                // Vaults
                TransactionEventType.ChangeVaultOwnerEvent => new[] {TransactionLogType.ChangeVaultOwnerLog},
                TransactionEventType.CreateVaultCertificateEvent => new[] {TransactionLogType.CreateVaultCertificateLog},
                TransactionEventType.RedeemVaultCertificateEvent => new[] {TransactionLogType.RedeemVaultCertificateLog},
                TransactionEventType.RevokeVaultCertificateEvent => new[] {TransactionLogType.RevokeVaultCertificateLog},
                _ => throw new ArgumentOutOfRangeException(nameof(eventType), "Invalid transaction event type")
            };
        }
    }
}
