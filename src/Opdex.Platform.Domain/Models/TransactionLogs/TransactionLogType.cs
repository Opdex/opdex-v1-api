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
        StartMiningLog = 18,
        CollectMiningRewardsLog = 19,
        StopMiningLog = 20,
        EnableMiningLog = 21,

        // Mined token logs
        DistributionLog = 22,
        
        // Vault
        CreateVaultCertificateLog = 23,
        RevokeVaultCertificateLog = 24,
        RedeemVaultCertificateLog = 25,
        ChangeVaultOwnerLog = 26
    }
}