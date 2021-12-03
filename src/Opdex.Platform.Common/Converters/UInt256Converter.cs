using Newtonsoft.Json;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Common.Converters;

public class UInt256Converter : JsonConverter<UInt256>
{
    public override void WriteJson(JsonWriter writer, UInt256 value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString());
    }

    public override UInt256 ReadJson(JsonReader reader, Type objectType, UInt256 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return new UInt256((string)reader.Value);
    }
}