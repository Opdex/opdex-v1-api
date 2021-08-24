using Newtonsoft.Json;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Common.Converters
{
    public class FixedDecimalConverter : JsonConverter<FixedDecimal>
    {
        public override FixedDecimal ReadJson(JsonReader reader, Type objectType, FixedDecimal existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return FixedDecimal.Parse((string)reader.Value);
        }

        public override void WriteJson(JsonWriter writer, FixedDecimal value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
