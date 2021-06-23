namespace Opdex.Platform.Common
{
    public static class TokenConstants
    {
        public static class Cirrus
        {
            public const string Symbol = "CRS";
            public const string Address = "CRS";
            public const int Decimals = 8;
            public const ulong Sats = 100_000_000;
        }

        public static class Opdex
        {
            public const string Symbol = "ODX";
            public const int Decimals = 8;
            public const ulong Sats = 100_000_000;
        }

        public static class LiquidityPoolToken
        {
            public const int Decimals = 8;
            public const string Symbol = "OLPT";
            public const string Name = "Opdex Liquidity Pool Token";
            public const ulong Sats = 100_000_000;
        }
    }
}