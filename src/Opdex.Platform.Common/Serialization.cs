using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Opdex.Platform.Common.Converters;
using System.Collections.Generic;

namespace Opdex.Platform.Common
{
    public class Serialization
    {
        public static JsonSerializerSettings DefaultJsonSettings => new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new List<JsonConverter>
                {
                    new StringEnumConverter(),
                    new UInt128Converter(),
                    new UInt256Converter(),
                    new AddressConverter(),
                    new FixedDecimalConverter(),
                    new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ssK" }
                }
        };
    }
}
