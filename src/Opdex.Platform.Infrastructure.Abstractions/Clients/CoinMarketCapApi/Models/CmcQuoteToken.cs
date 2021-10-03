using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Models
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class CmcQuoteToken
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Slug { get; set; }
        public ulong CirculatingSupply { get; set; }
        public ulong TotalSupply { get; set; }
        public IDictionary<string, CmcQuotePrice> Quote { get; set; }
    }
}
