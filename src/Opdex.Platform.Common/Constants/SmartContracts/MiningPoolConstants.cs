namespace Opdex.Platform.Common.Constants.SmartContracts
{
    public static class MiningPoolConstants
    {
        public static class Properties
        {
            // Insert properties as we would need them for a local call
            // (e.g. get_PropertyName)
        }

        public static class Methods
        {
            public const string StartMining = "StartMining";
            public const string StopMining = "StopMining";
            public const string CollectRewards = "CollectMiningRewards";
            public const string GetBalance = "GetBalance";
        }
    }
}
