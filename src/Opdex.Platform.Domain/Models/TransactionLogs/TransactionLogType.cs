namespace Opdex.Platform.Domain.Models.TransactionLogs
{
    public enum TransactionLogType : uint
    {
        Unknown = 0,

        // Market deployer logs
        CreateMarketLog = 1,
        ChangeDeployerOwnerLog = 2,

        // Market logs
        CreateLiquidityPoolLog = 3,

        // Standard market logs
        ChangeMarketOwnerLog = 4,
        ChangeMarketPermissionLog = 5,

        // Liquidity pool logs
        MintLog = 6,
        BurnLog = 7,
        SwapLog = 8,
        ReservesLog = 9,
        ApprovalLog = 10,
        TransferLog = 11,

        // Standard pool logs
        ChangeMarketLog = 12,

        // Staking pool logs
        StakeLog = 13,
        CollectStakingRewardsLog = 14,

        // Mining governance logs
        RewardMiningPoolLog = 15,
        NominationLog = 16,

        // Mining pool logs
        MineLog = 17,
        CollectMiningRewardsLog = 18,
        EnableMiningLog = 19,

        // Mined token logs
        DistributionLog = 20,

        // Vault
        CreateVaultCertificateLog = 21,
        RevokeVaultCertificateLog = 22,
        RedeemVaultCertificateLog = 23,
        ChangeVaultOwnerLog = 24
    }
}
