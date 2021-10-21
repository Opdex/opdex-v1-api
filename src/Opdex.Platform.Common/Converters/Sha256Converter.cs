using Newtonsoft.Json;
using Opdex.Platform.Common.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Opdex.Platform.Common.Converters
{
    public class Sha256Converter : JsonConverter<Sha256>
    {
        public override Sha256 ReadJson(JsonReader reader, Type objectType, [AllowNull] Sha256 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (Sha256.TryParse(reader.Value?.ToString(), out var hash)) return hash;
            throw new JsonException("Invalid Sha256 hash.");
        }

        public override void WriteJson(JsonWriter writer, Sha256 value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }

    public class NullableSha256Converter : JsonConverter<Sha256?>
    {
        public override Sha256? ReadJson(JsonReader reader, Type objectType, [AllowNull] Sha256? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (Sha256.TryParse(reader.Value?.ToString(), out var hash)) return hash;
            throw new JsonException("Invalid Sha256 hash.");
        }

        public override void WriteJson(JsonWriter writer, [AllowNull] Sha256? value, JsonSerializer serializer)
        {
            writer.WriteValue(value?.ToString());
        }
    }
}
