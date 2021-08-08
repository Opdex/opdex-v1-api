using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Models
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class CmcQuotePrice
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