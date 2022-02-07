using FluentAssertions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Mappers;

public class TransactionPlatformInfrastructureMapperProfileTests : PlatformInfrastructureMapperProfileTests
{
    [Theory]
    [InlineData(TransactionEventType.ApprovalEvent, TransactionLogType.ApprovalLog)]
    [InlineData(TransactionEventType.DistributionEvent, TransactionLogType.DistributionLog)]
    [InlineData(TransactionEventType.NominationEvent, TransactionLogType.NominationLog)]
    [InlineData(TransactionEventType.SwapEvent, TransactionLogType.SwapLog)]
    [InlineData(TransactionEventType.TransferEvent, TransactionLogType.TransferLog)]
    [InlineData(TransactionEventType.AddLiquidityEvent, TransactionLogType.MintLog)]
    [InlineData(TransactionEventType.CreateMarketEvent, TransactionLogType.CreateMarketLog)]
    [InlineData(TransactionEventType.EnableMiningEvent, TransactionLogType.EnableMiningLog)]
    [InlineData(TransactionEventType.OwnershipTransferredEvent, TransactionLogType.OwnershipTransferredLog)]
    [InlineData(TransactionEventType.RemoveLiquidityEvent, TransactionLogType.BurnLog)]
    [InlineData(TransactionEventType.StartMiningEvent, TransactionLogType.StartMiningLog)]
    [InlineData(TransactionEventType.StartStakingEvent, TransactionLogType.StartStakingLog)]
    [InlineData(TransactionEventType.StopMiningEvent, TransactionLogType.StopMiningLog)]
    [InlineData(TransactionEventType.StopStakingEvent, TransactionLogType.StopStakingLog)]
    [InlineData(TransactionEventType.SupplyChangeEvent, TransactionLogType.SupplyChangeLog)]
    [InlineData(TransactionEventType.ChangeMarketPermissionEvent, TransactionLogType.ChangeMarketPermissionLog)]
    [InlineData(TransactionEventType.CollectMiningRewardsEvent, TransactionLogType.CollectMiningRewardsLog)]
    [InlineData(TransactionEventType.CollectStakingRewardsEvent, TransactionLogType.CollectStakingRewardsLog)]
    [InlineData(TransactionEventType.CompleteVaultProposalEvent, TransactionLogType.CompleteVaultProposalLog)]
    [InlineData(TransactionEventType.CreateLiquidityPoolEvent, TransactionLogType.CreateLiquidityPoolLog)]
    [InlineData(TransactionEventType.CreateVaultCertificateEvent, TransactionLogType.CreateVaultCertificateLog)]
    [InlineData(TransactionEventType.CreateVaultProposalEvent, TransactionLogType.CreateVaultProposalLog)]
    [InlineData(TransactionEventType.RedeemVaultCertificateEvent, TransactionLogType.RedeemVaultCertificateLog)]
    [InlineData(TransactionEventType.RevokeVaultCertificateEvent, TransactionLogType.RevokeVaultCertificateLog)]
    [InlineData(TransactionEventType.RewardMiningPoolEvent, TransactionLogType.RewardMiningPoolLog)]
    [InlineData(TransactionEventType.VaultProposalPledgeEvent, TransactionLogType.VaultProposalPledgeLog)]
    [InlineData(TransactionEventType.VaultProposalVoteEvent, TransactionLogType.VaultProposalVoteLog)]
    [InlineData(TransactionEventType.ClaimPendingDeployerOwnershipEvent, TransactionLogType.ClaimPendingDeployerOwnershipLog)]
    [InlineData(TransactionEventType.ClaimPendingMarketOwnershipEvent, TransactionLogType.ClaimPendingMarketOwnershipLog)]
    [InlineData(TransactionEventType.SetPendingDeployerOwnershipEvent, TransactionLogType.SetPendingDeployerOwnershipLog)]
    [InlineData(TransactionEventType.SetPendingMarketOwnershipEvent, TransactionLogType.SetPendingMarketOwnershipLog)]
    [InlineData(TransactionEventType.VaultProposalWithdrawPledgeEvent, TransactionLogType.VaultProposalWithdrawPledgeLog)]
    [InlineData(TransactionEventType.VaultProposalWithdrawVoteEvent, TransactionLogType.VaultProposalWithdrawVoteLog)]
    public void From_TransactionEventType_To_TransactionLogType(TransactionEventType eventType, TransactionLogType logType)
    {
        Mapper.Map<TransactionLogType>(eventType).Should().Be(logType);
    }
}
