using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;

namespace Opdex.Platform.Common.Enums;

/// <summary>
/// Applied to an enum with members that are decorated with <see cref="EnumMemberAttribute" />, to keep model binding happy.
/// </summary>
/// <typeparam name="T">The enum type.</typeparam>
public class EnumMemberConverter<T> : EnumConverter
{
    public EnumMemberConverter(Type type) : base(type)
    { }

    public override object ConvertFrom(ITypeDescriptorContext context,
                                       CultureInfo culture,
                                       object value)
    {
        var type = typeof(T);

        foreach (var field in type.GetFields())
        {
            if (Attribute.GetCustomAttribute(field, typeof(EnumMemberAttribute)) is EnumMemberAttribute attribute
                && value is string enumValue
                && attribute.Value == enumValue)
            {
                return field.GetValue(null);
            }
        }

        return base.ConvertFrom(context, culture, value);
    }
}