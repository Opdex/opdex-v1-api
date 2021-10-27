using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Models
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class CmcLatestQuote
    {
        public CmcStatus Status { get; set; }
        public IDictionary<string, LatestQuoteToken> Data { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class LatestQuoteToken
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Slug { get; set; }
        public ulong CirculatingSupply { get; set; }
        public ulong TotalSupply { get; set; }
        public IDictionary<string, LatestQuotePrice> Quote { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class LatestQuotePrice
    {
        public decimal Price { get; set; }
        public decimal Volume24H { get; set; }
        public decimal PercentChange1H { get; set; }
        public decimal PercentChange24H { get; set; }
        public decimal PercentChange7D { get; set; }
        public decimal PercentChange30D { get; set; }
        public decimal PercentChange60D { get; set; }
        public decimal PercentChange90D { get; set; }
        public decimal MarketCap { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
