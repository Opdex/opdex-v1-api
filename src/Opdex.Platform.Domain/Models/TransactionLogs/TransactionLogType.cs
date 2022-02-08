namespace Opdex.Platform.Domain.Models.TransactionLogs;

public enum TransactionLogType : uint
{
    // Market deployer logs
    CreateMarketLog = 1,
    SetPendingDeployerOwnershipLog = 2,
    ClaimPendingDeployerOwnershipLog = 3,

    // Market logs
    CreateLiquidityPoolLog = 4,

    // Standard market logs
    SetPendingMarketOwnershipLog = 5,
    ClaimPendingMarketOwnershipLog = 6,
    ChangeMarketPermissionLog = 7,

    // Liquidity pool logs
    MintLog = 8,
    BurnLog = 9,
    SwapLog = 10,
    ReservesLog = 11,

    // Standard token logs
    ApprovalLog = 12,
    TransferLog = 13,

    // Staking pool logs
    StartStakingLog = 14,
    StopStakingLog = 15,
    CollectStakingRewardsLog = 16,

    // Mining governance logs
    RewardMiningPoolLog = 17,
    NominationLog = 18,

    // Mining pool logs
    StartMiningLog = 19,
    StopMiningLog = 20,
    CollectMiningRewardsLog = 21,
    EnableMiningLog = 22,

    // Mined token logs
    DistributionLog = 23,

    // Vault logs
    CreateVaultCertificateLog = 24,
    RevokeVaultCertificateLog = 25,
    RedeemVaultCertificateLog = 26,
    CreateVaultProposalLog = 27,
    CompleteVaultProposalLog = 28,
    VaultProposalPledgeLog = 29,
    VaultProposalWithdrawPledgeLog = 30,
    VaultProposalVoteLog = 31,
    VaultProposalWithdrawVoteLog = 32,

    // Interflux token logs,
    OwnershipTransferredLog = 33,
    SupplyChangeLog = 34,
}
