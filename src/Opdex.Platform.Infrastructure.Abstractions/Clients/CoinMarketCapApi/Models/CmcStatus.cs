using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Models
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class CmcStatus
    {
        public DateTime Timestamp { get; set; }
        public long ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public long Elapsed { get; set; }
        public long CreditCount { get; set; }
        public string Notice { get; set; }
    }
}