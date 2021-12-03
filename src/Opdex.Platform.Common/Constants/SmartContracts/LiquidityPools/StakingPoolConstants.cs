namespace Opdex.Platform.Common.Constants.SmartContracts.LiquidityPools;

public static class StakingPoolConstants
{
    public static class StateKeys
    {
        public const string StakingToken = "PN";
        public const string MiningPool = "PO";
        public const string TotalStaked = "PP";
        public const string StakingRewardsBalance = "PQ";
        public const string RewardPerStakedTokenLast = "PR";
        public const string ApplicableStakingRewards = "PS";
        public const string RewardPerStakedToken = "PT";
        public const string Reward = "PU";
        public const string StakedBalance = "PV";
    }

    public static class Properties
    {
        public const string StakingToken = nameof(StateKeys.StakingToken);
        public const string MiningPool = nameof(StateKeys.MiningPool);
        public const string TotalStaked = nameof(StateKeys.TotalStaked);
        public const string StakingRewardsBalance = nameof(StateKeys.StakingRewardsBalance);
        public const string RewardPerStakedTokenLast = nameof(StateKeys.RewardPerStakedTokenLast);
        public const string ApplicableStakingRewards = nameof(StateKeys.ApplicableStakingRewards);
        public const string RewardPerStakedToken = nameof(StateKeys.RewardPerStakedToken);
        public const string Reward = nameof(StateKeys.Reward);
        public const string StakedBalance = nameof(StateKeys.StakedBalance);
    }

    public static class Methods
    {
        public const string GetStakedBalance = "GetStakedBalance";
    }
}