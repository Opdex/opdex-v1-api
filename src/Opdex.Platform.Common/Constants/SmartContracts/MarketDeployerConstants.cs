namespace Opdex.Platform.Common.Constants.SmartContracts;

public static class MarketDeployerConstants
{
    public static class StateKeys
    {
        public const string Owner = "DA";
        public const string PendingOwner = "DB";
    }

    public static class Methods
    {
        public const string CreateStandardMarket = "CreateStandardMarket";
        public const string CreateStakingMarket = "CreateStakingMarket";
    }
}