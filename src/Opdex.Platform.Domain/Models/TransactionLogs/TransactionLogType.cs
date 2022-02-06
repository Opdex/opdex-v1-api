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

    // Interflux token logs,
    OwnershipTransferredLog = 14,
    SupplyChangeLog = 15,

    // Staking pool logs
    StartStakingLog = 16,
    StopStakingLog = 17,
    CollectStakingRewardsLog = 18,

    // Mining governance logs
    RewardMiningPoolLog = 19,
    NominationLog = 20,

    // Mining pool logs
    StartMiningLog = 21,
    StopMiningLog = 22,
    CollectMiningRewardsLog = 23,
    EnableMiningLog = 24,

    // Mined token logs
    DistributionLog = 25,

    // Vault logs
    CreateVaultCertificateLog = 26,
    RevokeVaultCertificateLog = 27,
    RedeemVaultCertificateLog = 28,
    CreateVaultProposalLog = 29,
    CompleteVaultProposalLog = 30,
    VaultProposalPledgeLog = 31,
    VaultProposalWithdrawPledgeLog = 32,
    VaultProposalVoteLog = 33,
    VaultProposalWithdrawVoteLog = 34
}
