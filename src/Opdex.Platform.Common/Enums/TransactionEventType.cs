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
    ReservesChangeEvent = 11,
    StartStakingEvent = 12,
    StopStakingEvent = 13,
    CollectStakingRewardsEvent = 14,

    // Mining pool logs
    StartMiningEvent = 15,
    StopMiningEvent = 16,
    CollectMiningRewardsEvent = 17,
    EnableMiningEvent = 18,

    // Tokens
    ApprovalEvent = 19,
    TransferEvent = 20,
    DistributionEvent = 21,

    // Mining governance logs
    RewardMiningPoolEvent = 22,
    NominationEvent = 23,

    // Vault
    CreateVaultCertificateEvent = 24,
    RevokeVaultCertificateEvent = 25,
    RedeemVaultCertificateEvent = 26,

    // Vault
    CreateVaultProposalEvent = 27,
    CompleteVaultProposalEvent = 28,
    VaultProposalPledgeEvent = 29,
    VaultProposalWithdrawPledgeEvent = 30,
    VaultProposalVoteEvent = 31,
    VaultProposalWithdrawVoteEvent = 32,

    // Interflux tokens
    SetInterfluxCustodianEvent = 33,
    SupplyChangeEvent = 34,
}
