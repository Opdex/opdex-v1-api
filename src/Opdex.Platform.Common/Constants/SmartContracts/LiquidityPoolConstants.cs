namespace Opdex.Platform.Common.Constants.SmartContracts
{
    public class LiquidityPoolConstants
    {
        public static class Properties
        {
            // Insert properties as we would need them for a local call
            // (e.g. get_PropertyName)
        }

        public static class Methods
        {
            public const string CreateLiquidityPool = "CreatePool";
            public const string Sync = "Sync";
            public const string Skim = "Skim";
            public const string StartStaking = "StartStaking";
            public const string StopStaking = "StopStaking";
            public const string CollectStakingRewards = "CollectStakingRewards";
            public const string AddLiquidity = "AddLiquidity";
            public const string RemoveLiquidity = "RemoveLiquidity";
        }
    }
}
