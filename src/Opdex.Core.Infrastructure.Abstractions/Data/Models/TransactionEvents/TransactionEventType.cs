namespace Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents
{
    public enum TransactionEventType
    {
        Unknown = 0,
        // Controller
        PoolCreatedEvent = 1,
        
        // Liquidity Pool Events
        MintEvent = 2,
        BurnEvent = 3,
        SwapEvent = 4,
        SyncEvent = 5,
        ApprovalEvent = 6,
        TransferEvent = 7,
        
        // Staking Pool Events
        StakeEvent = 8,
        CollectStakingRewardsEvent = 9,
        UnstakeEvent = 10,
        
        // Mining Pool Events
        MineEvent = 11,
        CollectMiningRewardsEvent = 12,
        ExitMineEvent = 13,
        
        // Mining Governance Events
        MiningPoolCreatedEvent = 14,
        MiningPoolRewardedEvent = 15,
        NominationEvent = 16,
        MiningPoolRewardedAddedEvent = 17,
        
        // OPDX Events
        OwnerChangeEvent = 18,
        DistributionEvent = 19
    }
}