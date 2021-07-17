namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents
{
    public enum TransactionEventType
    {
        // Market deployer logs
        CreateMarketEvent = 1,
        ChangeDeployerOwnerEvent = 2,

        // Market logs
        CreateLiquidityPoolEvent = 3,
        ChangeMarketOwnerEvent = 4,
        ChangeMarketPermissionEvent = 5,

        // Liquidity pool logs
        ProvideEvent = 6,
        SwapEvent = 7,
        StakeEvent = 8,
        CollectStakingRewardsEvent = 9,

        // Mining pool logs
        MineEvent = 10,
        CollectMiningRewardsEvent = 11,
        EnableMiningEvent = 12,

        // Tokens
        ApprovalEvent = 13,
        TransferEvent = 14,
        DistributionEvent = 15,

        // Mining governance logs
        RewardMiningPoolEvent = 16,
        NominationEvent = 17,

        // Vault
        CreateVaultCertificateEvent = 18,
        RevokeVaultCertificateEvent = 19,
        RedeemVaultCertificateEvent = 20,
        ChangeVaultOwnerEvent = 21
    }
}
