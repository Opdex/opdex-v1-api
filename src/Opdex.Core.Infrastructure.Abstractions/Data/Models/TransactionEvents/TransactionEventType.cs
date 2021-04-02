namespace Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents
{
    public enum TransactionEventType
    {
        Unknown = 0,
        PoolCreatedEvent = 1,
        MintEvent = 2,
        BurnEvent = 3,
        SwapEvent = 4,
        SyncEvent = 5,
        ApprovalEvent = 6,
        TransferEvent = 7,
        StakeEvent = 8,
        CollectStakingRewardsEvent = 9,
        UnstakeEvent = 10,
        MineEvent = 11,
        CollectMiningRewardsEvent = 12,
        ExitMineEvent = 13
    }
}