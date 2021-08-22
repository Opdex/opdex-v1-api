using Newtonsoft.Json;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Common.Converters
{
    public class AddressConverter : JsonConverter<Address>
    {
        public override void WriteJson(JsonWriter writer, Address value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override Address ReadJson(JsonReader reader, Type objectType, Address existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return new Address(reader.Value?.ToString());
        }
    }
}
