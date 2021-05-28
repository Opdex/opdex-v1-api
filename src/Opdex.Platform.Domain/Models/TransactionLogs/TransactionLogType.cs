namespace Opdex.Platform.Domain.Models.TransactionLogs
{
    public enum TransactionLogType
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
        StartStakingLog = 13,
        CollectStakingRewardsLog = 14,
        StopStakingLog = 15,

        // Mining governance logs
        RewardMiningPoolLog = 16,
        NominationLog = 17,
        
        // Mining pool logs
        MineLog = 18,
        CollectMiningRewardsLog = 19,
        EnableMiningLog = 20,

        // Mined token logs
        DistributionLog = 21,
        
        // Vault
        CreateVaultCertificateLog = 22,
        RevokeVaultCertificateLog = 23,
        RedeemVaultCertificateLog = 24,
        ChangeVaultOwnerLog = 25
    }
}