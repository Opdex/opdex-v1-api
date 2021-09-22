namespace Opdex.Platform.Common.Constants.SmartContracts.LiquidityPools
{
    public static class LiquidityPoolConstants
    {
        public static class StateKeys
        {
            public const string TotalSupply = "PA";
            public const string TransactionFee = "PB";
            public const string Token = "PC";
            public const string ReserveCrs = "PD";
            public const string ReserveSrc = "PE";
            public const string KLast = "PF";
            public const string Locked = "PG";
            public const string Balance = "PH";
            public const string Allowance = "PI";
        }

        public static class Properties
        {
            public const string TotalSupply = nameof(StateKeys.TotalSupply);
            public const string TransactionFee = nameof(StateKeys.TransactionFee);
            public const string Token = nameof(StateKeys.Token);
            public const string ReserveCrs = nameof(StateKeys.ReserveCrs);
            public const string ReserveSrc = nameof(StateKeys.ReserveSrc);
            public const string KLast = nameof(StateKeys.KLast);
            public const string Locked = nameof(StateKeys.Locked);
            public const string Balance = nameof(StateKeys.Balance);
            public const string Allowance = nameof(StateKeys.Allowance);
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
