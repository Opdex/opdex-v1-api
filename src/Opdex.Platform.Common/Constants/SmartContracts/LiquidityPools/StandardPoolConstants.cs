namespace Opdex.Platform.Common.Constants.SmartContracts.LiquidityPools
{
    public class StandardPoolConstants
    {
        public static class StateKeys
        {
            public const string Market = "PJ";
            public const string AuthProviders = "PK";
            public const string AuthTraders = "PL";
            public const string MarketFeeEnabled = "PM";
        }

        public static class Properties
        {
            public const string Market = nameof(StateKeys.Market);
            public const string AuthProviders = nameof(StateKeys.AuthProviders);
            public const string AuthTraders = nameof(StateKeys.AuthTraders);
            public const string MarketFeeEnabled = nameof(StateKeys.MarketFeeEnabled);
        }

        public static class Methods
        {
            public const string Sync = "Sync";
            public const string Skim = "Skim";
            public const string StartStaking = "StartStaking";
            public const string StopStaking = "StopStaking";
            public const string CollectStakingRewards = "CollectStakingRewards";
        }
    }
}
