namespace Opdex.Platform.Common.Constants.SmartContracts
{
    public static class MiningPoolConstants
    {
        public static class StateKeys
        {
            public const string MiningGovernance = "PW";
            public const string MinedToken = "PX";
            public const string MiningPeriodEndBlock = "PY";
            public const string RewardRate = "PZ";
            public const string MiningDuration = "PAA";
            public const string LastUpdateBlock = "PAB";
        }

        public static class Properties
        {
            public const string MiningGovernance = nameof(MiningGovernance);
            public const string MinedToken = nameof(MinedToken);
            public const string MiningPeriodEndBlock = nameof(MiningPeriodEndBlock);
            public const string RewardRate = nameof(RewardRate);
            public const string MiningDuration = nameof(MiningDuration);
            public const string LastUpdateBlock = nameof(LastUpdateBlock);
        }

        public static class Methods
        {
            public const string StartMining = "StartMining";
            public const string StopMining = "StopMining";
            public const string CollectRewards = "CollectMiningRewards";
            public const string GetBalance = "GetBalance";
            public const string GetRewardPerStakedToken = "GetRewardPerStakedToken";
        }
    }
}
