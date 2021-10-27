using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Models
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class CmcHistoricalQuote
    {
        public CmcStatus Status { get; set; }
        public HistoricalQuoteData Data { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class HistoricalQuoteData
    {
        public IEnumerable<HistoricalQuoteTimeframe> Quotes { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public int IsActive { get; set; }
        public int IsFiat { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class HistoricalQuoteTimeframe
    {
        public DateTime Timestamp { get; set; }
        public IDictionary<string, HistoricalQuotePrice> Quote { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class HistoricalQuotePrice
    {
        public decimal Price { get; set; }
        public decimal Volume24H { get; set; }
        public decimal MarketCap { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
