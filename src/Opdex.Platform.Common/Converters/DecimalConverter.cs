using Newtonsoft.Json;
using System;

namespace Opdex.Platform.Common.Converters;

public class DecimalConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(decimal));
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteValue(((decimal)value).ToString("0.00000000"));
    }

    public override bool CanRead
    {
        get { return false; }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return reader.ReadAsDecimal();
    }
}
