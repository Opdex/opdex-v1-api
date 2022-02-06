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

    // Interflux tokens
    OwnershipTransferredEvent = 21,
    SupplyChangeEvent = 22,

    // Mining governance logs
    RewardMiningPoolEvent = 23,
    NominationEvent = 24,

    // Vault
    CreateVaultCertificateEvent = 25,
    RevokeVaultCertificateEvent = 26,
    RedeemVaultCertificateEvent = 27,

    // Vault
    CreateVaultProposalEvent = 28,
    CompleteVaultProposalEvent = 29,
    VaultProposalPledgeEvent = 30,
    VaultProposalWithdrawPledgeEvent = 31,
    VaultProposalVoteEvent = 32,
    VaultProposalWithdrawVoteEvent = 33
}
