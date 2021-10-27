namespace Opdex.Platform.Infrastructure.Clients.CoinMarketCapApi
{
    public static class CmcUriHelper
    {
        public static class Quotes
        {
            public const string Latest = "quotes/latest?id={0}";
            public const string Historical = "quotes/historical?id={0}&time_start={1}&interval=5m&count=1";
        }
    }
}
