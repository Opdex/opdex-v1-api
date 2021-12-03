namespace Opdex.Platform.Common.Constants.SmartContracts.Tokens;

public static class StakingTokenConstants
{
    // Important to note that IStandard256 Stratis token contracts, IOpdexMinedToken, and IOpdexLiquidityPool token contracts all use
    // different state keys to represent the base properties of the token.
    public static class StateKeys
    {
        public const string Symbol = "TA";
        public const string Name = "TB";
        public const string Decimals = "TC";
        public const string TotalSupply = "TD";
        public const string Creator = "TE";
        public const string MiningGovernance = "TF";
        public const string Vault = "TG";
        public const string VaultSchedule = "TH";
        public const string MiningSchedule = "TI";
        public const string Genesis = "TJ";
        public const string PeriodIndex = "TK";
        public const string PeriodDuration = "TL";
        public const string Balance = "TM";
        public const string Allowance = "TN";
        public const string NextDistributionBlock = "TO";
    }

    public static class Properties
    {
        public const string Symbol = nameof(StateKeys.Symbol);
        public const string Name = nameof(StateKeys.Name);
        public const string Decimals = nameof(StateKeys.Decimals);
        public const string TotalSupply = nameof(StateKeys.TotalSupply);
        public const string Creator = nameof(StateKeys.Creator);
        public const string MiningGovernance = nameof(StateKeys.MiningGovernance);
        public const string Vault = nameof(StateKeys.Vault);
        public const string VaultSchedule = nameof(StateKeys.VaultSchedule);
        public const string MiningSchedule = nameof(StateKeys.MiningSchedule);
        public const string Genesis = nameof(StateKeys.Genesis);
        public const string PeriodIndex = nameof(StateKeys.PeriodIndex);
        public const string PeriodDuration = nameof(StateKeys.PeriodDuration);
        public const string Balance = nameof(StateKeys.Balance);
        public const string Allowance = nameof(StateKeys.Allowance);
        public const string NextDistributionBlock = nameof(StateKeys.NextDistributionBlock);
    }

    public static class Methods
    {
        public const string Distribute = "Distribute";
    }
}