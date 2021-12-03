using Newtonsoft.Json;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Common.Converters;

public class UInt128Converter : JsonConverter<UInt128>
{
    public override void WriteJson(JsonWriter writer, UInt128 value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString());
    }

    public override UInt128 ReadJson(JsonReader reader, Type objectType, UInt128 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return new UInt128((string)reader.Value);
    }
}