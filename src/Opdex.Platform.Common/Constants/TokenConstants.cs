namespace Opdex.Platform.Common.Constants;

public static class TokenConstants
{
    public static class Cirrus
    {
        public const string Symbol = "CRS";
        public const string Name = "Cirrus";
        public const int Decimals = 8;
        public const ulong Sats = 100_000_000; // 100 Million Sats
        public const ulong TotalSupply = 10_000_000_000_000_000; // 100 Million Tokens
    }

    public static class Opdex
    {
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
