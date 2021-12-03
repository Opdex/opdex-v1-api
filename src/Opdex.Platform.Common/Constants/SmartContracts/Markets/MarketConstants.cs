namespace Opdex.Platform.Common.Constants.SmartContracts.Markets;

public static class MarketConstants
{
    public static class StateKeys
    {
        public const string TransactionFee = "MA";
    }

    public static class Properties
    {
        public const string TransactionFee = nameof(StateKeys.TransactionFee);
    }

    public static class Methods
    {
        public const string CreateLiquidityPool = "CreatePool";
    }
}