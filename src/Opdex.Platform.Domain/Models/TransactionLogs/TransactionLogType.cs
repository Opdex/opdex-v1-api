namespace Opdex.Platform.Domain.Models.TransactionLogs
{
    public enum TransactionLogType
    {
        Unknown = 0,
        
        // Market deployer logs 
        MarketCreatedLog = 1,
        
        // Market logs
        LiquidityPoolCreatedLog = 2,
        
        // Standard market logs
        MarketOwnerChangeLog = 3,
        PermissionsChangeLog = 4,
        
        // Liquidity pool logs
        MintLog = 5,
        BurnLog = 6,
        SwapLog = 7,
        ReservesLog = 8,
        ApprovalLog = 9,
        TransferLog = 10,
        
        // Standard pool logs
        MarketChangeLog = 11,
        
        // Staking pool logs
        StartStakingLog = 12,
        CollectStakingRewardsLog = 13,
        StopStakingLog = 14,

        // Mining governance logs
        MiningPoolCreatedLog = 15,
        RewardMiningPoolLog = 16,
        NominationLog = 17,
        
        // Mining pool logs
        StartMiningLog = 18,
        CollectMiningRewardsLog = 19,
        StopMiningLog = 20,
        MiningPoolRewardedLog = 21,

        // Mined token logs
        OwnerChangeLog = 22,
        DistributionLog = 23
    }
}