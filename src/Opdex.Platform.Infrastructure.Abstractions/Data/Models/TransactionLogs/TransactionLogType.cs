namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.TransactionLogs
{
    public enum TransactionLogType
    {
        Unknown = 0,
        // Controller
        LiquidityPoolCreatedLog = 1,
        
        // Liquidity LiquidityPool Logs
        MintLog = 2,
        BurnLog = 3,
        SwapLog = 4,
        ReservesLog = 5,
        ApprovalLog = 6,
        TransferLog = 7,
        
        // Staking LiquidityPool Logs
        EnterStakingPoolLog = 8,
        CollectStakingRewardsLog = 9,
        ExitStakingPoolLog = 10,

        // Mining Governance Logs
        MiningPoolCreatedLog = 11,
        RewardMiningPoolLog = 12,
        NominationLog = 13,
        
        // Mining LiquidityPool Logs
        EnterMiningPoolLog = 14,
        CollectMiningRewardsLog = 15,
        ExitMiningPoolLog = 16,
        MiningPoolRewardedLog = 17,

        // OPDX Logs
        OwnerChangeLog = 18,
        DistributionLog = 19
    }
}