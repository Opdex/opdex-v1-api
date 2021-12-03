namespace Opdex.Platform.Common.Enums;

public enum TransactionEventType
{
    // Market deployer logs
    CreateMarketEvent = 1,
    SetPendingDeployerOwnershipEvent = 2,
    ClaimPendingDeployerOwnershipEvent = 3,

    // Market logs
    CreateLiquidityPoolEvent = 4,
    SetPendingMarketOwnershipEvent = 5,
    ClaimPendingMarketOwnershipEvent = 6,
    ChangeMarketPermissionEvent = 7,

    // Liquidity pool logs
    AddLiquidityEvent = 8,
    RemoveLiquidityEvent = 9,
    SwapEvent = 10,
    StartStakingEvent = 11,
    StopStakingEvent = 12,
    CollectStakingRewardsEvent = 13,

    // Mining pool logs
    StartMiningEvent = 14,
    StopMiningEvent = 15,
    CollectMiningRewardsEvent = 16,
    EnableMiningEvent = 17,

    // Tokens
    ApprovalEvent = 18,
    TransferEvent = 19,
    DistributionEvent = 20,

    // Mining governance logs
    RewardMiningPoolEvent = 21,
    NominationEvent = 22,

    // Vault
    CreateVaultCertificateEvent = 23,
    RevokeVaultCertificateEvent = 24,
    RedeemVaultCertificateEvent = 25,
    SetPendingVaultOwnershipEvent = 26,
    ClaimPendingVaultOwnershipEvent = 27
}