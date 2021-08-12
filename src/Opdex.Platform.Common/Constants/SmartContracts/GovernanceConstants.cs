namespace Opdex.Platform.Common.Constants.SmartContracts
{
    public static class GovernanceConstants
    {
        public const uint MiningPoolsFundedPerYear = 48;
        public const uint MaxNominations = 4;

        public static class Properties
        {
            // Insert properties as we would need them for a local call
            // (e.g. get_PropertyName)
        }

        public static class Methods
        {
            public const string RewardMiningPools = "RewardMiningPools";
            public const string RewardMiningPool = "RewardMiningPool";
        }
    }
}
