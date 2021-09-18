namespace Opdex.Platform.Common.Constants.SmartContracts
{
    public static class GovernanceConstants
    {
        public const uint MiningPoolsFundedPerYear = 48;
        public const uint MaxNominations = 4;

        public static class StateKeys
        {
            public const string MinedToken = "GA";
            public const string Notified = "GB";
            public const string Nominations = "GC";
            public const string MiningDuration = "GD";
            public const string NominationPeriodEnd = "GE";
            public const string MiningPoolsFunded = "GF";
            public const string MiningPoolReward = "GG";
            public const string Locked = "GH";
            public const string MiningPool = "GI";
        }

        public static class Properties
        {
            public const string MinedToken = nameof(StateKeys.MinedToken);
            public const string Notified = nameof(StateKeys.Notified);
            public const string Nominations = nameof(StateKeys.Nominations);
            public const string MiningDuration = nameof(StateKeys.MiningDuration);
            public const string NominationPeriodEnd = nameof(StateKeys.NominationPeriodEnd);
            public const string MiningPoolsFunded = nameof(StateKeys.MiningPoolsFunded);
            public const string MiningPoolReward = nameof(StateKeys.MiningPoolReward);
            public const string Locked = nameof(StateKeys.Locked);
            public const string MiningPool = nameof(StateKeys.MiningPool);
        }

        public static class Methods
        {
            public const string RewardMiningPools = "RewardMiningPools";
            public const string RewardMiningPool = "RewardMiningPool";
        }
    }
}
