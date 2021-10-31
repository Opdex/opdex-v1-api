using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Opdex.Platform.Common.Converters;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Serialization
{
    public static class StratisFullNode
    {
        public static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new List<JsonConverter>
                {
                    new AddressConverter(),
                    new UInt128Converter(),
                    new UInt256Converter(),
                    new Sha256Converter(),
                    new NullableSha256Converter(),
                    new SmartContractMethodParameterConverter()
                }
        };
    }
}
