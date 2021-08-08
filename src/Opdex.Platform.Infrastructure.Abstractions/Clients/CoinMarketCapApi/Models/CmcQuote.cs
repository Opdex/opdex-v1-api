using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Models
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class CmcQuote
    {
        public CmcStatus Status { get; set; }
        public IDictionary<string, CmcQuoteToken> Data { get; set; }
    }
}