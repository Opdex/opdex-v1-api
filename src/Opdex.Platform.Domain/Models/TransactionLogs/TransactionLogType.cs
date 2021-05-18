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
        CreateMiningPoolLog = 16,
        RewardMiningPoolLog = 17,
        NominationLog = 18,
        
        // Mining pool logs
        StartMiningLog = 19,
        CollectMiningRewardsLog = 20,
        StopMiningLog = 21,
        EnableMiningLog = 22,

        // Mined token logs
        DistributionLog = 23,
        
        // Vault
        CreateVaultCertificateLog = 24,
        RevokeVaultCertificateLog = 25,
        RedeemVaultCertificateLog = 26,
        ChangeVaultOwnerLog = 27
    }
}