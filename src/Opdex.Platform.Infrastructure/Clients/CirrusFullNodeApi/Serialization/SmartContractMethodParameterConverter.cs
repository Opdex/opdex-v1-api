using Newtonsoft.Json;
using Opdex.Platform.Domain.Models.Transactions;
using System;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Serialization
{
    public class SmartContractMethodParameterConverter : JsonConverter<SmartContractMethodParameter>
    {
        public override SmartContractMethodParameter ReadJson(JsonReader reader, Type objectType, SmartContractMethodParameter existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return SmartContractMethodParameter.Deserialize((string)reader.Value);
        }

        public override void WriteJson(JsonWriter writer, SmartContractMethodParameter value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Serialize());
        }
    }
}
