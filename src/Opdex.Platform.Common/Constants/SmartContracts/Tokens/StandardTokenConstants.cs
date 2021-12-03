namespace Opdex.Platform.Common.Constants.SmartContracts.Tokens;

// Important to note that IStandard256 Stratis token contracts, IOpdexMinedToken, and IOpdexLiquidityPool token contracts all use
// different state keys to represent the base properties of the token.
public static class StandardTokenConstants
{
    public static class StateKeys
    {
        public const string Symbol = "Symbol";
        public const string Name = "Name";
        public const string Decimals = "Decimals";
        public const string TotalSupply = "TotalSupply";
    }

    public static class Properties
    {
        public const string Symbol = nameof(StateKeys.Symbol);
        public const string Name = nameof(StateKeys.Name);
        public const string Decimals = nameof(StateKeys.Decimals);
        public const string TotalSupply = nameof(StateKeys.TotalSupply);
    }

    public static class Methods
    {
        public const string Approve = "Approve";
        public const string Allowance = "Allowance";
        public const string GetBalance = "GetBalance";
    }
}