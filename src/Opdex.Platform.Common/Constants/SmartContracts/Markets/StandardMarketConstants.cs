namespace Opdex.Platform.Common.Constants.SmartContracts.Markets
{
    public static class StandardMarketConstants
    {
        public static class StateKeys
        {
            public const string AuthTraders = "MB";
            public const string AuthProviders = "MC";
            public const string AuthPoolCreators = "MD";
            public const string IsAuthorized = "ME";
            public const string MarketFeeEnabled = "MF";
            public const string Owner = "MG";
            public const string PendingOwner = "MH";
            public const string Pool = "MI";
        }

        public static class Properties
        {
            public const string AuthTraders = nameof(StateKeys.AuthTraders);
            public const string AuthProviders = nameof(StateKeys.AuthProviders);
            public const string AuthPoolCreators = nameof(StateKeys.AuthPoolCreators);
            public const string IsAuthorized = nameof(StateKeys.IsAuthorized);
            public const string MarketFeeEnabled = nameof(StateKeys.MarketFeeEnabled);
            public const string Owner = nameof(StateKeys.Owner);
            public const string PendingOwner = nameof(StateKeys.PendingOwner);
            public const string Pool = nameof(StateKeys.Pool);
        }

        public static class Methods
        {
            public const string Authorize = "Authorize";
            public const string IsAuthorized = "IsAuthorized";
            public const string SetPendingOwnership = "SetPendingOwnership";
            public const string ClaimPendingOwnership = "ClaimPendingOwnership";
            public const string CollectMarketFees = "CollectMarketFees";
        }
    }
}
