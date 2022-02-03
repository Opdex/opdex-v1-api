using Newtonsoft.Json;
using Opdex.Platform.Common.Models;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Opdex.Platform.Common.Converters;

public class AddressConverter : JsonConverter<Address>
{
    public override void WriteJson(JsonWriter writer, Address value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString());
    }

    public override Address ReadJson(JsonReader reader, Type objectType, Address existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (Address.TryParse(reader.Value?.ToString(), out var address)) return address;
        throw new JsonException("Invalid address.");
    }
}

public class AddressTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        return value is string address ? new Address(address) : base.ConvertFrom(context, culture, value);
    }
}
